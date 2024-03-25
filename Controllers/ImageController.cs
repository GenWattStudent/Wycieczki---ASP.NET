using Book.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class ImageController : Controller
{
    private readonly ImageService _imageService;
    private readonly TourService _tourService;

    public ImageController(ImageService imageService, TourService tourService)
    {
        _imageService = imageService;
        _tourService = tourService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(int id, int tourId)
    {
        await _imageService.DeleteImage(id);
        var tour = await _tourService.GetTour(tourId);
        return RedirectToAction("EditTour", "Tour", tour);
    }

}