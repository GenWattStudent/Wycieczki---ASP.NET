using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Book.App.Models;
using Book.App.ViewModels;
using Book.App.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace Book.App.Controllers;

[Authorize]
public class TourController : Controller
{
    private readonly ILogger<TourController> _logger;
    private readonly TourService _tourService;

    public TourController(ILogger<TourController> logger, TourService tourService)
    {
        _logger = logger;
        _tourService = tourService;
    }

    public async Task<IActionResult> Tours()
    {
        _logger.LogInformation("Getting all tours");
        var tours = await _tourService.GetTours();
        return View(tours);
    }

    public IActionResult AddTour()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddTour(string name, string description, decimal price, IFormFile image, int maxUsers, DateTime startDate, DateTime endDate, string waypoints)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) || price <= 0)
        {
            return BadRequest();
        }

        var imageUrl = string.Empty;

        if (image != null)
        {
            _tourService.SaveImage(image, out imageUrl);
        }

        var tour = new TourModel
        {
            Name = name,
            Description = description,
            ImageUrl = imageUrl,
            Price = price,
            StartDate = startDate,
            EndDate = endDate,
            MaxUsers = maxUsers
        };

        var waypointsList = string.IsNullOrEmpty(waypoints) ? new List<AddTourWaypointsModel>() : JsonConvert.DeserializeObject<List<AddTourWaypointsModel>>(waypoints);

        await _tourService.AddTour(tour, waypointsList);

        return RedirectToAction("Tours");
    }

    public async Task<IActionResult> TourDetails(int id)
    {
        var tour = await _tourService.GetTour(id);
        if (tour == null)
        {
            return RedirectToAction("Tours");
        }
        return View(tour);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTour(int id)
    {
        var tour = await _tourService.GetTour(id);
        if (tour == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(tour.ImageUrl))
        {
            _tourService.RemoveImage(tour.ImageUrl);
        }

        await _tourService.DeleteTour(id);

        return RedirectToAction("Tours");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
