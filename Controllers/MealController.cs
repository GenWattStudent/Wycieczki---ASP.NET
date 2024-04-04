using Book.App.Models;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class MealController : Controller
{
    private readonly ITourService _tourService;
    private readonly IMealService _mealService;

    public
    MealController(ITourService tourService, IMealService mealService)
    {
        _mealService = mealService;
        _tourService = tourService;
    }

    public async Task<IActionResult> Edit(int id)
    {
        var tour = await _tourService.GetById(id);
        var mealViewModel = new MealViewModel
        {
            TourModel = tour,
            MealModel = new MealModel { TourId = id }
        };

        return View(mealViewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int tourId, MealModel mealModel)
    {
        var tour = await _tourService.GetById(tourId);

        if (tour == null)
        {
            ViewData["Error"] = "Tour not found";
            return RedirectToAction("Edit", new { id = tourId });
        }

        mealModel.TourId = tourId;
        await _mealService.Add(mealModel);

        return RedirectToAction("Edit", new { id = tourId });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var meal = await _mealService.Delete(id);
        return RedirectToAction("Edit", new { id = meal.TourId });
    }
}