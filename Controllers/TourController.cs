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
    public async Task<IActionResult> AddTour(AddTourModel addTourModel)
    {
        List<string> imageUrls = new List<string>();

        try
        {
            if (ModelState.IsValid == false)
            {
                return View();
            }

            if (addTourModel.Images != null && addTourModel.Images.Count > 0)
            {
                _tourService.SaveImages(addTourModel.Images, out imageUrls);
            }

            var tour = new TourModel
            {
                Name = addTourModel.Name,
                Description = addTourModel.Description,
                Price = addTourModel.Price,
                StartDate = addTourModel.StartDate,
                EndDate = addTourModel.EndDate,
                MaxUsers = addTourModel.MaxUsers,
            };

            var waypointsList = string.IsNullOrEmpty(addTourModel.Waypoints) ? new List<AddTourWaypointsModel>() : JsonConvert.DeserializeObject<List<AddTourWaypointsModel>>(addTourModel.Waypoints);

            await _tourService.AddTour(tour, waypointsList, imageUrls);

            return RedirectToAction("Tours");
        }
        catch (Exception)
        {
            return View();
        }
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
    public async Task<IActionResult> DeleteImage(int id)
    {
        try
        {
            await _tourService.DeleteImage(id);
            return RedirectToAction("EditTour");
        }
        catch (Exception)
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
