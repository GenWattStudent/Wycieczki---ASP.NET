using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.ViewComponents;

public class TourNavViewComponent : ViewComponent
{
    private readonly ITourService _tourService;

    public TourNavViewComponent(ITourService tourService)
    {
        _tourService = tourService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int tourId, bool isEdit = false)
    {
        var tour = await _tourService.GetById(tourId);
        var tourNavViewModel = new TourNavViewModel
        {
            TourModel = tour,
            IsEdit = isEdit
        };

        return View(tourNavViewModel);
    }
}
