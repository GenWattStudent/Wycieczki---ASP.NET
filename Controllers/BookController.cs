using System.Security.Claims;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly BookService _bookService;

    public BookController(BookService tourService)
    {
        _bookService = tourService;
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var tour = await _bookService.GetTourById(int.Parse(userId), id);

        if (tour == null)
        {
            return RedirectToAction("Index");
        }

        var bookViewModel = _bookService.GetBookViewModel(tour);

        return View(bookViewModel);
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var userTours = await _bookService.GetToursByUserId(int.Parse(userId));
        var booksViewModel = new BooksViewModel();

        foreach (var tour in userTours)
        {
            booksViewModel.Books.Add(_bookService.GetBookViewModel(tour));
        }

        return View(booksViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> BookTour(int id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _bookService.AddTourToUser(int.Parse(userId), id);

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = e.Message;
            return RedirectToAction("TourDetails", "Tour", new { id = id });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await _bookService.DeleteCancelTour(int.Parse(userId), id);

            return RedirectToAction("Index");
        }
        catch (Exception)
        {
            return RedirectToAction("Index");
        }
    }
}