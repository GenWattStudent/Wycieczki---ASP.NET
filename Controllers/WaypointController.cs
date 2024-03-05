using Book.App.Models;
using Book.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class WaypointController : Controller
{
    private readonly WaypointService _waypointService;


    public WaypointController(WaypointService waypointService)
    {
        _waypointService = waypointService;
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
            return NotFound();
        }

        await _waypointService.AddImages(images, waypoint);
        Console.WriteLine(waypoint.TourId);
        return RedirectToAction("EditTour", "Tour", new { id = waypoint.TourId });
    }
}