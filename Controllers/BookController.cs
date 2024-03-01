using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class BookController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> BookTour(int id)
    {
        return View();
    }
}