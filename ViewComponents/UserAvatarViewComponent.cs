using System.Security.Claims;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.ViewComponents;

public class UserAvatarViewComponent : ViewComponent
{
    private readonly IUserService _userService;

    public UserAvatarViewComponent(IUserService userManager)
    {
        _userService = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var username = UserClaimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _userService.GetByUsername(username ?? "");

        if (user == null)
        {
            return View(new NavViewModel());
        }

        var viewModel = new NavViewModel
        {
            Username = user.Username,
            AgencyId = user.TravelAgencyId ?? 0,
            ImagePath = user.ImagePath ?? ""
        };

        return View(viewModel);
    }
}