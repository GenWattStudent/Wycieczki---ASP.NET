using Book.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class MealController : Controller
{
    private readonly ITourService _tourService;

    public MealController(ITourService tourService)
    {
        _tourService = tourService;
    }

    public async Task<IActionResult> Edit(int id)
    {
        var tour = await _tourService.GetTour(id);
        return View(tour);
    }
}