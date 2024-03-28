using Book.App.Models;
using Book.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.ViewComponents;

public class TourNavViewComponent : ViewComponent
{
    private readonly TourService _tourService;

    public TourNavViewComponent(TourService tourService)
    {
        _tourService = tourService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int tourId)
    {
        // var model = new FilterModel();
        Console.WriteLine("TourNavViewComponent: " + tourId);
        var tour = await _tourService.GetTour(tourId);
        return View(tour);
    }
}
