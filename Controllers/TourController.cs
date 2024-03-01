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
    public async Task<IActionResult> AddTour(string name, string description, decimal price, IFormFile image)
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
            Price = price
        };

        await _tourService.AddTour(tour);

        return RedirectToAction("Tours");
    }

    public async Task<IActionResult> TourDetails(int id)
    {
        var tour = await _tourService.GetTour(id);
        if (tour == null)
        {
            return NotFound();
        }
        return View(tour);
    }

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
