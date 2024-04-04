using Book.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class ImageController : Controller
{
    private readonly IImageService _imageService;
    private readonly ITourService _tourService;

    public ImageController(IImageService imageService, ITourService tourService)
    {
        _imageService = imageService;
        _tourService = tourService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(int id, int tourId)
    {
        await _imageService.Delete(id);
        var tour = await _tourService.GetById(tourId);
        return RedirectToAction("EditTour", "Tour", tour);
    }

}