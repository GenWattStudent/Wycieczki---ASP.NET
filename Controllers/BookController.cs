using System.Security.Claims;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly BookService _bookService;
    private readonly GeoService _geoService;

    public BookController(BookService tourService, GeoService geoService)
    {
        _bookService = tourService;
        _geoService = geoService;
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var tour = await _bookService.GetTourById(int.Parse(userId), id);

        if (tour == null)
        {
            return RedirectToAction("Index");
        }

        var tourViewModel = new TourViewModel
        {
            TourModel = tour,
            Distance = _geoService.CalculateDistance(tour.Waypoints),
        };

        // the trip started and not ended calculate dsitance and which waypoint you are
        if (tour.StartDate <= DateTime.Now && tour.EndDate >= DateTime.Now)
        {
            tourViewModel.PercentOfTime = (DateTime.Now - tour.StartDate).TotalDays / (tour.EndDate - tour.StartDate).TotalDays;
            var nextWaypointData = _geoService.CalculateDistanceToNextWaypoint(tour.Waypoints, tourViewModel.PercentOfTime);
            tourViewModel.DistanceToNextWaypoint = nextWaypointData.Distance;
            tourViewModel.TourLat = nextWaypointData.TourLat;
            tourViewModel.TourLon = nextWaypointData.TourLon;
            tourViewModel.NextWaypoint = nextWaypointData.NextWaypoint;
        }
        else if (tour.StartDate > DateTime.Now)
        {
            tourViewModel.PercentOfTime = 0;
        }
        else
        {
            tourViewModel.PercentOfTime = 1;
        }

        return View(tourViewModel);
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var userTours = await _bookService.GetToursByUserId(int.Parse(userId));
        var toursViewModel = new ToursViewModel
        {
            Tours = userTours
        };

        return View(toursViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> BookTour(int id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _bookService.AddTourToUser(int.Parse(userId), id);

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            TempData["ErrorMessage"] = e.Message;
            return RedirectToAction("TourDetails", "Tour", new { id = id });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _bookService.DeleteCancelTour(int.Parse(userId), id);

            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            return RedirectToAction("Index");
        }
    }
}