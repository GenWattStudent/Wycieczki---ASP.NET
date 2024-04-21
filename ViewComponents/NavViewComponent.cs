using System.Security.Claims;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.ViewComponents;

public class NavViewComponent : ViewComponent
{
    private readonly IUserService _userService;

    public NavViewComponent(IUserService userService)
    {
        _userService = userService;
    }

    public IViewComponentResult Invoke()
    {
        var username = UserClaimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        var user = _userService.GetByUsername(username ?? "").Result;

        if (user == null)
        {
            return View(new NavViewModel());
        }

        var navViewModel = new NavViewModel
        {
            Username = user.Username,
            AgencyId = user.TravelAgencyId ?? 0
        };

        return View(navViewModel);
    }
}