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
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize]
    public async Task<IActionResult> Tours(FilterModel filterModel)
    {
        var tours = await _tourService.Get(filterModel);
        return View(tours);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult AddTour()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTour(AddTourViewModel addTourModel)
    {
        if (!ModelState.IsValid)
        {
            var errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return Json(new { error = errorMessage });
        }

        var tour = new TourModel(addTourModel);
        await _tourService.Add(tour);

        return RedirectToAction("EditTour", new { id = tour.Id });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTour(EditTourViewModel editTourModel)
    {
        var tour = new TourModel(editTourModel);
        if (!ModelState.IsValid)
        {
            return View(tour);
        }

        await _tourService.Edit(tour, editTourModel);

        return RedirectToAction("Tours");
    }

    public async Task<IActionResult> TourDetails(int id)
    {
        var tour = await _tourService.GetById(id);
        if (tour == null)
        {
            return RedirectToAction("Tours");
        }
        return View(tour);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTour(int id)
    {
        var tour = await _tourService.GetById(id);
        if (tour == null)
        {
            return NotFound();
        }

        await _tourService.Delete(id);
        return Redirect("/Tour/Tours");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditTour(int id)
    {
        var tour = await _tourService.GetById(id);

        if (tour == null)
        {
            return RedirectToAction("AddTour");
        }
        return View(tour);
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
