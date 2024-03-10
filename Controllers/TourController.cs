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
    public async Task<IActionResult> AddTour([FromForm] AddTourModel addTourModel)
    {
        List<string> imageUrls = new List<string>();
        Console.WriteLine(addTourModel.Waypoints[0].Type + "asdasdasdsadasd");
        var tour = new TourModel
        {
            Name = addTourModel.Name,
            Description = addTourModel.Description,
            Price = addTourModel.Price,
            StartDate = addTourModel.StartDate,
            EndDate = addTourModel.EndDate,
            MaxUsers = addTourModel.MaxUsers,
        };

        if (!ModelState.IsValid)
        {
            return BadRequest();
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

        if (ModelState.IsValid == false)
        {

            return View(editTourModel);
        }

        if (editTourModel.Images != null && editTourModel.Images.Count > 0)
        {
            _tourService.SaveImages(editTourModel.Images, _tourFolder, out imageUrls);
        }

        var tour = new TourModel
        {
            Id = editTourModel.Id,
            Name = editTourModel.Name,
            Description = editTourModel.Description,
            Price = editTourModel.Price,
            StartDate = editTourModel.StartDate,
            EndDate = editTourModel.EndDate,
            MaxUsers = editTourModel.MaxUsers,
        };

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

        return RedirectToAction("Tours");
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
        try
        {
            await _tourService.DeleteImage(id);
            var tour = await _tourService.GetTour(tourId);
            Console.WriteLine(tour.Images.Count);
            return View("EditTour", tour);
        }
        catch (Exception e)
        {
            return View();
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
