using System.Security.Claims;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.ViewComponents;

public class MealTimelineViewComponent : ViewComponent
{
    private readonly IReservationService _reservationService;

    public MealTimelineViewComponent(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    public IViewComponentResult Invoke()
    {
        var userId = int.Parse(UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

        if (userId == 0)
        {
            return View(new TimelineViewModel());
        }

        var tour = _reservationService.GetClosestReservation(userId).Result.Tour;
        var timelineViewModel = new TimelineViewModel() { Meals = tour.Meals, TourId = tour.Id };

        return View(timelineViewModel);
    }
}