using Book.App.Models;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class WaypointController : Controller
{
    private readonly IWaypointService _waypointService;
    private readonly ITourService _tourService;
    private readonly IWeatherService _weatherService;

    public WaypointController(IWaypointService waypointService, IWeatherService weatherService, ITourService tourService)
    {
        _waypointService = waypointService;
        _weatherService = weatherService;
        _tourService = tourService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditWaypoints(int tourId, int waypointId)
    {
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
            CurrentWaypoint = tour.Waypoints.FirstOrDefault(w => w.Id == waypointId)
        };
        return View(editWaypointsViewModel);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var waypoint = await _waypointService.Get(id);
        Console.WriteLine(waypoint.Id);
        if (waypoint == null)
        {
            TempData["ErrorMessage"] = "Waypoint not found";
            return RedirectToAction("Tours", "Tour");
        }

        await _waypointService.Delete(waypoint);
        return RedirectToAction("Edit", new { id = waypoint.TourId });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(WaypointModel waypoint)
    {
        Console.WriteLine("EDIT HAHAHA");
        var waypointDb = await _waypointService.Edit(waypoint);

        if (waypointDb == null)
        {
            TempData["ErrorMessage"] = "Waypoint not found";
            return RedirectToAction("EditTour", "Tour", new { id = waypoint.TourId });
        }
        Console.WriteLine(waypointDb.Id + " " + waypointDb.TourId);
        return RedirectToAction("EditWaypoints", new { tourId = waypointDb.TourId, waypointId = waypointDb.Id });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddImages(List<IFormFile> images, int id)
    {
        var waypoint = await _waypointService.Get(id);

        if (waypoint == null)
        {
            TempData["ErrorMessage"] = "Waypoint not found";
            var referrer = Request.Headers["Referer"].ToString();
            return Redirect(referrer);
        }

        await _waypointService.AddImages(images, waypoint);

        return RedirectToAction("EditTour", "Tour", new { id = waypoint.TourId });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var tour = await _tourService.GetById(id);
        return View(tour);
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var waypoint = await _waypointService.Get(id);
        if (waypoint == null)
        {
            TempData["ErrorMessage"] = "Waypoint not found";

            var referrer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referrer))
            {
                return RedirectToAction("Tours", "Tour");
            }
            return Redirect(referrer);
        }

        var weather = await _weatherService.Get(waypoint.Latitude, waypoint.Longitude);
        var waypointViewModel = new WaypointViewModel
        {
            Waypoint = waypoint,
            Weather = weather
        };
        return View(waypointViewModel);
    }

    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Add(List<AddTourWaypointsViewModel> addTourWaypointsModel, int tourId)
    {
        Console.WriteLine("ADD WAYPOINT " + addTourWaypointsModel[0].Description == null);
        await _waypointService.Add(addTourWaypointsModel, tourId);
        return RedirectToAction("Edit", new { id = tourId });
    }
}