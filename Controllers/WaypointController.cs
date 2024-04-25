using Book.App.Filters.Exception;
using Book.App.Helpers;
using Book.App.Models;
using Book.App.Services;
using Book.App.ViewModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Book.App.Controllers;

[ServiceFilter(typeof(NotInAgencyExceptionFilter))]
[EnableRateLimiting("fixed")]
public class WaypointController : Controller
{
    private readonly IWaypointService _waypointService;
    private readonly ITourService _tourService;
    private readonly IWeatherService _weatherService;
    private readonly IReservationService _reservationService;
    private readonly IAgencyService _agencyService;
    private readonly IValidator<WaypointModel> _editWaypointValidator;

    public WaypointController(IWaypointService waypointService, IWeatherService weatherService, ITourService tourService, IReservationService reservationService, IAgencyService agencyService, IValidator<WaypointModel> editWaypointValidator)
    {
        _waypointService = waypointService;
        _weatherService = weatherService;
        _tourService = tourService;
        _reservationService = reservationService;
        _agencyService = agencyService;
        _editWaypointValidator = editWaypointValidator;
    }

    [Authorize(Roles = "AgencyAdmin")]
    public async Task<IActionResult> EditWaypoints(int tourId, int waypointId, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        var tour = await _tourService.GetById(tourId);

        if (tour == null)
        {
            TempData["ErrorMessage"] = "Tour not found";
            return RedirectToAction("Tours", "Tour");
        }

        var editWaypointsViewModel = new EditWaypointsViewModel
        {
            TourId = tourId,
            Waypoints = tour.Waypoints,
            CurrentWaypoint = tour.Waypoints.FirstOrDefault(w => w.Id == waypointId),
            TourModel = tour
        };

        return View(editWaypointsViewModel);
    }

    [Authorize(Roles = "Admin,AgencyAdmin")]
    public async Task<IActionResult> Delete(int id, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        var waypoint = await _waypointService.Get(id);

        if (waypoint == null)
        {
            TempData["ErrorMessage"] = "Waypoint not found";
            return RedirectToAction("Tours", "Tour");
        }

        await _waypointService.Delete(waypoint);
        return RedirectToAction("Edit", new { id = waypoint.TourId, agencyId });
    }

    [Authorize(Roles = "AgencyAdmin,Admin")]
    [HttpPost]
    public async Task<IActionResult> EditWaypoints(WaypointModel waypoint, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();

        var validationResult = _editWaypointValidator.Validate(waypoint);

        if (!validationResult.IsValid)
        {
            var tour = await _tourService.GetById(waypoint.TourId);
            if (tour == null)
            {
                TempData["ErrorMessage"] = "Tour not found";
                return RedirectToAction("Tours", "Agency", new { agencyId });
            }

            validationResult.AddToModelState(ModelState, null);

            return View(new EditWaypointsViewModel { CurrentWaypoint = tour.Waypoints.FirstOrDefault(w => w.Id == waypoint.Id), TourModel = tour, Waypoints = tour.Waypoints, TourId = waypoint.TourId });
        }

        var waypointDb = await _waypointService.Edit(waypoint);

        if (waypointDb == null)
        {
            TempData["ErrorMessage"] = "Waypoint not found";
            return RedirectToAction("EditTour", "Tour", new { id = waypoint.TourId, agencyId });
        }

        return RedirectToAction("EditWaypoints", new { tourId = waypointDb.TourId, waypointId = waypointDb.Id, agencyId });
    }

    [Authorize(Roles = "AgencyAdmin")]
    [HttpPost]
    public async Task<IActionResult> AddImages(List<IFormFile> images, int id, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        var waypoint = await _waypointService.Get(id);

        if (waypoint == null)
        {
            TempData["ErrorMessage"] = "Waypoint not found";
            return RedirectToAction("Tours", "Agency", new { agencyId });
        }

        await _waypointService.AddImages(images, waypoint);

        return RedirectToAction("EditTour", "Tour", new { id = waypoint.TourId, agencyId });
    }

    [Authorize(Roles = "Admin,AgencyAdmin")]
    public async Task<IActionResult> Edit(int id, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        var tour = await _tourService.GetById(id);

        if (tour == null)
        {
            TempData["ErrorMessage"] = "Tour not found";
            return RedirectToAction("Tours", "Agency", new { agencyId });
        }

        return View(tour);
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var waypoint = await _waypointService.Get(id);
        if (waypoint == null)
        {
            TempData["ErrorMessage"] = "Waypoint not found";
            return RedirectToAction("Tours", "Tour");
        }

        var weather = await _weatherService.Get(waypoint.Latitude, waypoint.Longitude);
        var waypointViewModel = new WaypointViewModel
        {
            Waypoint = waypoint,
            Weather = weather
        };
        return View(waypointViewModel);
    }

    [Authorize(Roles = "AgencyAdmin")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Add(List<AddTourWaypointsViewModel> addTourWaypointsModel, int tourId, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        await _waypointService.Add(addTourWaypointsModel, tourId);
        return RedirectToAction("Edit", new { id = tourId, agencyId });
    }

    [Authorize]
    public async Task<IActionResult> MapView(int id)
    {
        var tour = await _tourService.GetById(id);

        if (tour == null)
        {
            TempData["ErrorMessage"] = "Tour not found";
            return RedirectToAction("Tours", "Tour");
        }

        var bookViewModel = _reservationService.GetBookViewModel(tour);
        return View(bookViewModel);
    }

    [Authorize]
    public async Task<IActionResult> WaypointsDetails(int tourId)
    {
        var tour = await _tourService.GetById(tourId);
        return View(tour);
    }
}