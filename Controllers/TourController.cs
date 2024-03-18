using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Book.App.Models;
using Book.App.ViewModels;
using Book.App.Services;
using Microsoft.AspNetCore.Authorization;

namespace Book.App.Controllers;

[Authorize]
public class TourController : Controller
{
    private readonly ILogger<TourController> _logger;
    private readonly TourService _tourService;
    private readonly string _tourFolder = "Tours";

    public TourController(ILogger<TourController> logger, TourService tourService)
    {
        _logger = logger;
        _tourService = tourService;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize]
    public async Task<IActionResult> Tours()
    {
        _logger.LogInformation("Getting all tours");
        var tours = await _tourService.GetTours();
        return View(tours);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AddTour()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddTour([FromForm] AddTourModel addTourModel)
    {
        List<string> imageUrls = new List<string>();

        var tour = new TourModel(addTourModel);

        if (!ModelState.IsValid)
        {
            return View(tour);
        }

        if (addTourModel.Images != null && addTourModel.Images.Count > 0)
        {
            _tourService.SaveImages(addTourModel.Images, _tourFolder, out imageUrls);
        }

        await _tourService.AddTour(tour, addTourModel, imageUrls);

        return RedirectToAction("Tours");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditTour(EditTourModel editTourModel)
    {
        List<string> imageUrls = new List<string>();
        var tour = new TourModel(editTourModel);

        if (ModelState.IsValid == false)
        {
            return View(tour);
        }

        if (editTourModel.Images != null && editTourModel.Images.Count > 0)
        {
            _tourService.SaveImages(editTourModel.Images, _tourFolder, out imageUrls);
        }

        await _tourService.EditTour(tour, editTourModel, imageUrls);

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

        _tourService.DeleteTour(id);
        Console.WriteLine((await _tourService.GetTours()).Count);
        return Redirect("/Tour/Tours");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditTour(int id)
    {
        var tour = await _tourService.GetTour(id);
        if (tour == null)
        {
            return RedirectToAction("AddTour");
        }
        return View(tour);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(int id, int tourId)
    {
        await _tourService.DeleteImage(id);
        var tour = await _tourService.GetTour(tourId);
        Console.WriteLine((await _tourService.GetTours()).Count);
        return View("EditTour", tour);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult GetMapKey()
    {
        var mapTilerKey = Environment.GetEnvironmentVariable("MAP_TILER_KEY");
        return Json(new { key = mapTilerKey });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Active()
    {
        var tours = await _tourService.GetActiveTours();
        return View(tours);
    }
}
