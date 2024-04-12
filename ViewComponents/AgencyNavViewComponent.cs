using System.Security.Claims;
using Book.App.Services;
using Book.App.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Book.App.ViewComponents;

public class AgencyNavViewComponent : ViewComponent
{
    private readonly IAgencyService _agencyService;

    public AgencyNavViewComponent(IAgencyService agencyService)
    {
        _agencyService = agencyService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int agencyId)
    {
        if (int.TryParse(UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
        {
            try
            {
                var agency = await _agencyService.GetByIdAsync(agencyId);
                var user = agency.Users.FirstOrDefault(u => u.Id == userId);

                if (user == null)
                {
                    return View(new AgencyNavViewModel());
                }

                var agencyNavViewModel = new AgencyNavViewModel { TravelAgency = agency, Roles = user.Roles };

                return View(agencyNavViewModel);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                View(new AgencyNavViewModel());
            }
        }

        return View(new AgencyNavViewModel());
    }
}