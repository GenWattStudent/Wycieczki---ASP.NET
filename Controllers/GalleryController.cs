using Book.App.Filters.Exception;
using Book.App.Helpers;
using Book.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

[ServiceFilter(typeof(NotInAgencyExceptionFilter))]
public class GalleryController : Controller
{
    private readonly ITourService _tourService;
    private readonly IImageService _imageService;
    private readonly IAgencyService _agencyService;

    public GalleryController(ITourService tourService, IImageService imageService, IAgencyService agencyService)
    {
        _tourService = tourService;
        _imageService = imageService;
        _agencyService = agencyService;
    }

    [Authorize(Roles = "Admin,AgencyAdmin")]
    // GET: Gallery{tourId}
    public async Task<IActionResult> Edit(int id, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        var tour = await _tourService.GetById(id);
        return View(tour);
    }

    [Authorize(Roles = "AgencyAdmin")]
    // POST: Gallery{tourId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(List<IFormFile> files, int id, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        await _imageService.AddImagesToTour(files, id);
        return RedirectToAction("Edit", new { id, agencyId });
    }

    [Authorize]
    // GET Details
    public async Task<IActionResult> Details(int id)
    {
        var tour = await _tourService.GetById(id);
        return View(tour);
    }

    [Authorize(Roles = "AgencyAdmin")]
    // POST: Gallery{waypointId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImageToWaypoint(List<IFormFile> files, int id, int tourId)
    {
        await _imageService.AddImagesToWaypoint(files, id);
        return RedirectToAction("EditWaypoints", "Waypoint", new { tourId, waypointId = id });
    }

    [Authorize(Roles = "Admin,AgencyAdmin")]
    // POST: Gallery/Delete{imageId}
    public async Task<IActionResult> Delete(int id, int tourId, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        await _imageService.Delete(id);
        return RedirectToAction("Edit", new { id = tourId, agencyId });
    }
}