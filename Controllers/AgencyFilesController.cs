using System.Security.Claims;
using Book.App.Filters.Exception;
using Book.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Book.App.Controllers;

[ServiceFilter(typeof(NotInAgencyExceptionFilter))]
[EnableRateLimiting("fixed")]
public class AgencyFilesController : Controller
{
    private readonly IAgencyService _agencyService;
    private readonly IAgencyFilesService _agencyFilesService;

    public AgencyFilesController(IAgencyService agencyService, IAgencyFilesService agencyFilesService)
    {
        _agencyService = agencyService;
        _agencyFilesService = agencyFilesService;
    }

    [Authorize(Roles = "AgencyAdmin")]
    public async Task<IActionResult> AddGallery(int agencyId)
    {
        try
        {
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                var agency = await _agencyService.GetByIdAsync(agencyId);
                if (agency.Users.Any(u => u.Id == userId))
                {
                    return View(agency);
                }
            }

            TempData["ErrorMessage"] = "You are not allowed to access this page";
            return RedirectToAction("UserDetails", "User");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Error";
            return RedirectToAction("UserDetails", "User");
        }
    }

    [Authorize(Roles = "AgencyAdmin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddGallery(int agencyId, List<IFormFile> files)
    {
        try
        {
            if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
            {
                var agency = await _agencyService.GetByIdAsync(agencyId);
                if (agency.Users.Any(u => u.Id == userId))
                {
                    await _agencyFilesService.AddGalleryAsync(agencyId, files);
                    return RedirectToAction("AddGallery", new { agencyId });
                }
            }

            TempData["ErrorMessage"] = "You are not allowed to access this page";
            return RedirectToAction("UserDetails", "User");
        }
        catch (Exception)
        {
            TempData["ErrorMessage"] = "Error";
            return RedirectToAction("UserDetails", "User");
        }
    }
}