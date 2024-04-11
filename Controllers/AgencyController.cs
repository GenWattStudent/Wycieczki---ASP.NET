
using System.Security.Claims;
using Book.App.Models;
using Book.App.Services;
using Book.App.ViewModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.Controllers;

public class AgencyController : Controller
{
    private readonly IValidator<TravelAgencyModel> _validator;
    private readonly IAgencyService _agencyService;

    public AgencyController(IAgencyService agencyService, IValidator<TravelAgencyModel> validator)
    {
        _agencyService = agencyService;
        _validator = validator;
    }

    [Authorize]
    public IActionResult Create()
    {
        var travelAgencyViewModel = new TravelAgencyViewModel() { TravelAgency = new TravelAgencyModel() };
        return View(travelAgencyViewModel);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TravelAgencyViewModel travelAgencyViewModel)
    {
        ValidationResult result = await _validator.ValidateAsync(travelAgencyViewModel?.TravelAgency);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState);
            return View(travelAgencyViewModel);
        }

        if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            try
            {
                await _agencyService.CreateAsync(travelAgencyViewModel, userId);
                return RedirectToAction("UserInfo", "User");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return View(travelAgencyViewModel);
            }
        }

        return RedirectToAction("Login", "User");
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Requests()
    {
        var agencyRequests = await _agencyService.GetNotAcceptedAsync();
        var travelAgencyListViewModel = new TravelAgencyListViewModel() { TravelAgencies = agencyRequests };
        return View(travelAgencyListViewModel);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(int id, string reason)
    {
        await _agencyService.AcceptAsync(id, reason ?? "Accepted by admin");
        return RedirectToAction("Requests");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string reason)
    {
        await _agencyService.RejectAsync(id, reason ?? "Rejected by admin");
        return RedirectToAction("Requests");
    }

    [Authorize]
    public async Task<IActionResult> Leave(int userId)
    {
        try
        {
            await _agencyService.LeaveAsync(userId);
            return RedirectToAction("UserInfo", "User");
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = e.Message;
            return RedirectToAction("UserInfo", "User");
        }
    }
}