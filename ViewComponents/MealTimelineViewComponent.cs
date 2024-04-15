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
        if (int.TryParse(UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            var tour = _reservationService.GetClosestReservation(userId).Result?.Tour;

            if (tour == null)
            {
                return View(new TimelineViewModel());
            }

            var timelineViewModel = new TimelineViewModel() { Meals = tour.Meals, TourId = tour.Id };

            return View(timelineViewModel);
        }

        return View(new TimelineViewModel());
    }
}