using System.Security.Claims;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

[Authorize]
public class BookController : Controller
{
    private readonly IReservationService _bookService;

    public BookController(IReservationService tourService)
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

        var userReservations = await _bookService.GetUserActiveAndFutureReservations(int.Parse(userId));
        var booksViewModel = new BooksViewModel();

        foreach (var reservation in userReservations)
        {
            booksViewModel.Books.Add(_bookService.GetBookViewModel(reservation.Tour));
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
            return RedirectToAction("TourDetails", "Tour", new { id });
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

            await _bookService.Cancel(int.Parse(userId), id);

            return RedirectToAction("Index");
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = e.Message;
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> History()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var reservations = await _bookService.GetReservationsHistoryByUserId(int.Parse(userId));
        var booksViewModel = new BooksViewModel();

        foreach (var reservation in reservations)
        {
            booksViewModel.Books.Add(_bookService.GetBookViewModel(reservation.Tour));
        }

        return View(booksViewModel);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUserInTour(int id, int tourId)
    {
        try
        {
            await _bookService.Delete(id, tourId);

            return RedirectToAction("TourDetails", "Tour", new { id = tourId });
        }
        catch (Exception)
        {
            return RedirectToAction("Index");
        }
    }
}