using Book.App.Filters.Exception;
using Book.App.Helpers;
using Book.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Book.App.Controllers;

[ServiceFilter(typeof(NotInAgencyExceptionFilter))]
[EnableRateLimiting("fixed")]

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
    public async Task<IActionResult> AddImageToWaypoint(List<IFormFile> files, int id, int tourId, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        await _imageService.AddImagesToWaypoint(files, id);
        return RedirectToAction("EditWaypoints", "Waypoint", new { tourId, waypointId = id, agencyId });
    }

    [Authorize(Roles = "Admin,AgencyAdmin")]
    // POST: Gallery/Delete{imageId}
    public async Task<IActionResult> Delete(int id, int tourId, int agencyId, string redirect)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        await _imageService.Delete(id);

        if (!string.IsNullOrEmpty(redirect))
        {
            return Redirect(redirect);
        }

        return RedirectToAction(redirect ?? "Edit", new { id = tourId, agencyId });
    }
}