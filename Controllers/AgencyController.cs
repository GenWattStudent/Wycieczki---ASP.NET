
using System.Security.Claims;
using AutoMapper;
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
    private readonly IValidator<CreateAgencyViewModel> _validator;
    private readonly IAgencyService _agencyService;
    private readonly IMapper _mapper;

    public AgencyController(IAgencyService agencyService, IValidator<CreateAgencyViewModel> validator, IMapper mapper)
    {
        _agencyService = agencyService;
        _validator = validator;
        _mapper = mapper;
    }

    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateAgencyViewModel createAgencyViewModel)
    {
        ValidationResult result = await _validator.ValidateAsync(createAgencyViewModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState, null);
            return View(createAgencyViewModel);
        }

        if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            try
            {
                await _agencyService.CreateAsync(_mapper.Map<TravelAgencyModel>(createAgencyViewModel), userId);
                return RedirectToAction("UserInfo", "User");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return View(createAgencyViewModel);
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

    [Authorize]
    public async Task<IActionResult> Details(int agencyId)
    {
        try
        {
            var travelAgency = await _agencyService.GetByIdAsync(agencyId);
            return View(new TravelAgencyViewModel() { TravelAgency = travelAgency });
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = e.Message;
            return RedirectToAction("UserInfo", "User");
        }
    }

    [Authorize(Roles = "AgencyAdmin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(CreateAgencyViewModel createAgencyViewModel)
    {
        ValidationResult result = await _validator.ValidateAsync(createAgencyViewModel);

        if (!result.IsValid)
        {
            result.AddToModelState(ModelState, null);
            return View(createAgencyViewModel);
        }

        try
        {
            await _agencyService.UpdateAsync(_mapper.Map<TravelAgencyModel>(createAgencyViewModel));
            return RedirectToAction("Details", new { agencyId = createAgencyViewModel.Id });
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = e.Message;
            return View(createAgencyViewModel);
        }
    }

    [Authorize(Roles = "Admin,AgencyAdmin")]
    public async Task<IActionResult> Delete(int agencyId)
    {
        if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int currentUserId))
        {
            try
            {
                await _agencyService.DeleteAsync(agencyId, currentUserId);
                return RedirectToAction("UserInfo", "User");
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("UserInfo", "User");
            }
        }

        return RedirectToAction("Login", "User");
    }
}