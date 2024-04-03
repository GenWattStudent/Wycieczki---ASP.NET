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
    public async Task<IActionResult> Delete(int id)
    {
        var waypoint = await _waypointService.GetWaypoint(id);

        if (waypoint == null)
        {
            return NotFound();
        }

        await _waypointService.Delete(id);
        return RedirectToAction("EditTour", "Tour", new { id = waypoint.TourId });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(AddTourWaypointsModel addTourWaypointsModel)
    {
        var waypoint = await _waypointService.GetWaypoint(addTourWaypointsModel.Id);
        if (waypoint == null)
        {
            return NotFound();
        }

        await _waypointService.Edit(addTourWaypointsModel);
        return RedirectToAction("EditTour", "Tour", new { id = waypoint.TourId });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddImages(List<IFormFile> images, int id)
    {
        var waypoint = await _waypointService.GetWaypoint(id);
        Console.WriteLine(id);
        if (waypoint == null)
        {
            TempData["ErrorMessage"] = "Waypoint not found";
            var referrer = Request.Headers["Referer"].ToString();
            return Redirect(referrer);
        }

        await _waypointService.AddImages(images, waypoint);
        Console.WriteLine(waypoint.TourId);
        return RedirectToAction("EditTour", "Tour", new { id = waypoint.TourId });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var tour = await _tourService.GetTour(id);
        return View(tour);
    }

    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var waypoint = await _waypointService.GetWaypoint(id);
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

        var weather = await _weatherService.GetWeather(waypoint.Latitude, waypoint.Longitude);
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
    public async Task<IActionResult> Add(List<AddTourWaypointsModel> addTourWaypointsModel, int tourId)
    {
        await _waypointService.Add(addTourWaypointsModel, tourId);
        return RedirectToAction("Edit", new { id = tourId });
    }
}