using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Book.App.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Book.App.ViewModels;
using Book.App.Services;

namespace Book.App.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BookService _bookService;

    public HomeController(ILogger<HomeController> logger, BookService bookService)
    {
        _bookService = bookService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var closestTour = await _bookService.GetClosestTourByUserId(int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value));
        var homeViewModel = new HomeViewModel { Tour = closestTour };
        if (closestTour != null)
        {
            homeViewModel.Book = _bookService.GetBookViewModel(closestTour);
        }

        return View(homeViewModel);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
