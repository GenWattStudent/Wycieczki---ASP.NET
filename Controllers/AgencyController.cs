using System.Security.Claims;
using AutoMapper;
using Book.App.Filters.Exception;
using Book.App.Services;
using Book.App.ViewModels;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Book.App.Controllers;
[ServiceFilter(typeof(NotInAgencyExceptionFilter))]
[EnableRateLimiting("fixed")]
public class AgencyController : Controller
{
    private readonly IValidator<CreateAgencyViewModel> _validator;
    private readonly IAgencyService _agencyService;
    private readonly IMapper _mapper;
    private readonly ITourService _tourService;

    public AgencyController(IAgencyService agencyService, IValidator<CreateAgencyViewModel> validator, IMapper mapper, ITourService tourService)
    {
        _agencyService = agencyService;
        _validator = validator;
        _mapper = mapper;
        _tourService = tourService;
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
        Console.WriteLine(createAgencyViewModel.Name + " asdasddasd");
        if (!result.IsValid)
        {
            Console.WriteLine("Validation failed");
            result.AddToModelState(ModelState, null);
            return View("Create", createAgencyViewModel);
        }
        Console.WriteLine("Validation passed");
        if (int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId))
        {
            try
            {
                await _agencyService.CreateAsync(createAgencyViewModel, userId);
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
            return View(new TravelAgencyViewModel() { TravelAgency = travelAgency, CreateAgencyViewModel = _mapper.Map<CreateAgencyViewModel>(travelAgency) });
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
        try
        {
            ValidationResult result = await _validator.ValidateAsync(createAgencyViewModel);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError($"CreateAgencyViewModel.{error.PropertyName}", error.ErrorMessage);
                }
                return View("Details", new TravelAgencyViewModel() { TravelAgency = await _agencyService.GetByIdAsync(createAgencyViewModel.Id), CreateAgencyViewModel = createAgencyViewModel });
            }
            await _agencyService.UpdateAsync(createAgencyViewModel);
            return RedirectToAction("Details", new { agencyId = createAgencyViewModel.Id });
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = e.Message;
            return RedirectToAction("Details", new { agencyId = createAgencyViewModel.Id });
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

    [Authorize(Roles = "Admin,AgencyAdmin")]
    public async Task<IActionResult> DeleteLogo(int agencyId)
    {
        try
        {
            await _agencyService.DeleteLogoAsync(agencyId);
            return RedirectToAction("Details", new { agencyId });
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = e.Message;
            return RedirectToAction("Details", new { agencyId });
        }
    }

    [Authorize(Roles = "Admin,AgencyAdmin")]
    public async Task<IActionResult> DeleteVideo(int agencyId)
    {
        try
        {
            await _agencyService.DeleteVideoAsync(agencyId);
            return RedirectToAction("Details", new { agencyId });
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = e.Message;
            return RedirectToAction("Details", new { agencyId });
        }
    }

    [Authorize(Roles = "Admin,AgencyAdmin")]
    public async Task<IActionResult> Tours(int agencyId)
    {
        try
        {
            var tours = await _tourService.GetByAgencyIdAsync(agencyId);
            return View(new AgencyTourListViewModel() { Tours = tours, AgencyId = agencyId });
        }
        catch (Exception e)
        {
            TempData["ErrorMessage"] = e.Message;
            return RedirectToAction("Details", new { agencyId });
        }
    }
}
