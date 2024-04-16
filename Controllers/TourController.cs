using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Book.App.Models;
using Book.App.ViewModels;
using Book.App.Services;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Book.App.Helpers;
using Book.App.Filters.Exception;

namespace Book.App.Controllers;

[Authorize]
[ServiceFilter(typeof(NotInAgencyExceptionFilter))]
public class TourController : Controller
{
    private readonly ITourService _tourService;
    private readonly IAgencyService _agencyService;
    private readonly IMapper _mapper;
    private readonly IValidator<AddTourViewModel> _addTourValidator;

    public TourController(ITourService tourService, IAgencyService agencyService, IMapper mapper, IValidator<AddTourViewModel> addTourValidator)
    {
        _tourService = tourService;
        _agencyService = agencyService;
        _mapper = mapper;
        _addTourValidator = addTourValidator;
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Authorize]
    public async Task<IActionResult> Tours(FilterModel filterModel)
    {
        var tours = await _tourService.GetVisible(filterModel);
        return View(tours);
    }

    [Authorize(Roles = "AgencyAdmin")]
    public async Task<IActionResult> AddTour(int agencyId)
    {
        var agency = await _agencyService.GetByIdAsync(agencyId);
        return View(new CreateTourViewModel { AgencyModel = agency });
    }

    [HttpPost]
    [Authorize(Roles = "AgencyAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTour(AddTourViewModel addTourViewModel)
    {
        var validationResult = await _addTourValidator.ValidateAsync(addTourViewModel);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState, null);
            return View(addTourViewModel);
        }

        var tour = _mapper.Map<TourModel>(addTourViewModel);
        await _tourService.Add(tour);

        return RedirectToAction("EditTour", new { id = tour.Id });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,AgencyAdmin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditTour(FormTourViewModel formTourViewModel)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), formTourViewModel.AddTourViewModel.TravelAgencyId)) return NotFound();
        var addTourViewModel = formTourViewModel.AddTourViewModel;
        var validationResult = await _addTourValidator.ValidateAsync(addTourViewModel);

        if (!validationResult.IsValid)
        {
            validationResult.AddToModelState(ModelState, null);
            return View(new FormTourViewModel { TourModel = await _tourService.GetById(addTourViewModel.Id), AddTourViewModel = addTourViewModel });
        }

        await _tourService.Edit(addTourViewModel);

        return RedirectToAction("EditTour", new { id = addTourViewModel.Id });
    }

    public async Task<IActionResult> TourDetails(int id)
    {
        var tour = await _tourService.GetById(id);
        if (tour == null)
        {
            return RedirectToAction("Tours");
        }
        return View(tour);
    }

    [Authorize(Roles = "Admin,AgencyAdmin")]
    public async Task<IActionResult> DeleteTour(int id, int agencyId)
    {
        if (!await _agencyService.IsUserInAgencyAsync(this.GetCurrentUserId(), agencyId)) throw new NotInAgencyException();
        var tour = await _tourService.GetById(id);
        if (tour == null)
        {
            return NotFound();
        }

        await _tourService.Delete(id);
        return Redirect("/Tour/Tours");
    }

    [Authorize(Roles = "AgencyAdmin,Admin")]
    public async Task<IActionResult> EditTour(int id)
    {
        var tour = await _tourService.GetById(id);

        if (tour == null)
        {
            return RedirectToAction("AddTour");
        }
        return View(new FormTourViewModel { TourModel = tour, AddTourViewModel = _mapper.Map<AddTourViewModel>(tour), AgencyId = tour.TravelAgencyId });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult GetMapKey()
    {
        var mapTilerKey = Environment.GetEnvironmentVariable("MAP_TILER_KEY");
        return Json(new { key = mapTilerKey });
    }

    [Authorize(Roles = "AgecyAdmin")]
    public async Task<IActionResult> Active()
    {
        var tours = await _tourService.GetActiveTours();
        return View(tours);
    }
}
