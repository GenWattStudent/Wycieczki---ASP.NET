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

    [Authorize(Roles = "AgencyAdmin")]
    public async Task<IActionResult> Add(int tourId)
    {
        var tour = await _tourService.GetById(tourId);
        var mealViewModel = new MealViewModel
        {
            TourModel = tour,
            MealModel = new MealModel { TourId = tourId }
        };

        return View(mealViewModel);
    }

    [Authorize(Roles = "AgencyAdmin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(MealModel mealModel)
    {
        var tour = await _tourService.GetById(mealModel.TourId);

        if (tour == null)
        {
            ViewData["Error"] = "Tour not found";
            return RedirectToAction("Tours", "Tour");
        }

        await _mealService.Add(mealModel);

        return RedirectToAction("Add", new { tourId = mealModel.TourId });
    }

    [Authorize(Roles = "Admin,AgecyAdmin")]
    public async Task<IActionResult> Edit(int tourIdd, int mealId)
    {
        var tour = await _tourService.GetById(tourIdd);

        if (tour == null)
        {
            ViewData["Error"] = "Tour not found";
            return RedirectToAction("Tours", "Tour");
        }

        var mealViewModel = new MealViewModel
        {
            TourModel = tour,
            MealModel = tour.Meals.FirstOrDefault(m => m.Id == mealId)
        };

        return View(mealViewModel);
    }

    [Authorize]
    public async Task<IActionResult> Details(int tourId)
    {
        var tour = await _tourService.GetById(tourId);
        var mealViewModel = new MealViewModel
        {
            TourModel = tour,
            MealModel = new MealModel { TourId = tourId }
        };

        return View(mealViewModel);
    }

    [Authorize(Roles = "Admin,AgecyAdmin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(MealModel model)
    {
        var tour = await _tourService.GetById(model.TourId);

        if (tour == null)
        {
            ViewData["Error"] = "Tour not found";
            return RedirectToAction("Tours", "Tour");
        }

        var mealToUpdate = tour.Meals.FirstOrDefault(m => m.Id == model.Id);

        if (mealToUpdate == null)
        {
            ViewData["Error"] = "Meal not found";
            return RedirectToAction("Add", new { tourId = model.TourId });
        }

        mealToUpdate.Update(model);

        await _mealService.Update(mealToUpdate);

        return RedirectToAction("Add", new { tourId = model.TourId });
    }

    [Authorize(Roles = "Admin,AgecyAdmin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var meal = await _mealService.Delete(id);
        return RedirectToAction("Add", new { tourId = meal.TourId });
    }
}