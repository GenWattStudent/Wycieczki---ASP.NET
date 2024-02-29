using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Book.App.Models;
using Book.App.ViewModels;
using Book.App.Services;

namespace Book.App.Controllers;

public class TourController : Controller
{
    private readonly ILogger<TourController> _logger;
    private readonly TourService _tourService;

    public TourController(ILogger<TourController> logger, TourService tourService)
    {
        _logger = logger;
        _tourService = tourService;
    }

    public IActionResult Tours()
    {
        _logger.LogInformation("Getting all tours");
        return View(_tourService.GetTours());
    }

    public IActionResult AddTour()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddTour(string name, string description, decimal price, IFormFile image)
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

        var tour = new TourViewModel
        {
            Id = _tourService.GetTours().Max(t => t.Id) + 1,
            Name = name,
            Description = description,
            ImageUrl = imageUrl,
            Price = price
        };

        _tourService.AddTour(tour);

        return RedirectToAction("Tours");
    }

    public IActionResult TourDetails(int id)
    {
        var tour = _tourService.GetTour(id);
        if (tour == null)
        {
            return NotFound();
        }
        return View(tour);
    }

    public IActionResult DeleteTour(int id)
    {
        var tour = _tourService.GetTour(id);
        if (tour == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(tour.ImageUrl))
        {
            _tourService.RemoveImage(tour.ImageUrl);
        }

        _tourService.DeleteTour(id);

        return RedirectToAction("Tours");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
