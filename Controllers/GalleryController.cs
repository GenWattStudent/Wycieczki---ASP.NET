using Book.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class GalleryController : Controller
{
    private readonly ITourService _tourService;
    private readonly IImageService _imageService;

    public GalleryController(ITourService tourService, IImageService imageService)
    {
        _tourService = tourService;
        _imageService = imageService;
    }

    [Authorize(Roles = "Admin")]
    // GET: Gallery{tourId}
    public async Task<IActionResult> Edit(int id)
    {
        var tour = await _tourService.GetTour(id);
        return View(tour);
    }

    [Authorize(Roles = "Admin")]
    // POST: Gallery{tourId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(List<IFormFile> files, int id)
    {
        await _imageService.AddImagesToTour(files, id);
        return RedirectToAction("Edit", new { id });
    }

    [Authorize(Roles = "Admin")]
    // POST: Gallery/Delete{imageId}
    public async Task<IActionResult> Delete(int id, int tourId)
    {
        await _imageService.DeleteImage(id);
        return RedirectToAction("Edit", new { id = tourId });
    }
}