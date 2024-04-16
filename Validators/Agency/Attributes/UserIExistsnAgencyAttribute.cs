using System.Security.Claims;
using Book.App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Book.App.Validators.Agency.Attributes;

public class UserExistsInAgencyAttribute : ActionFilterAttribute
{
    private readonly IAgencyService _agencyService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserExistsInAgencyAttribute(IAgencyService agencyService, IHttpContextAccessor httpContextAccessor)
    {
        _agencyService = agencyService;
        _httpContextAccessor = httpContextAccessor;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Console.WriteLine("UserExistsInAgencyAttribute");

        object agencyIdObj;
        object travelAgencyObjectId;

        if (!context.ActionArguments.TryGetValue("AgencyId", out agencyIdObj))
        {
            agencyIdObj = null;
        }

        if (!context.ActionArguments.TryGetValue("TravelAgencyObjectId", out travelAgencyObjectId))
        {
            travelAgencyObjectId = null;
        }
        Console.WriteLine(agencyIdObj);
        if (agencyIdObj == null && travelAgencyObjectId == null)
        {
            context.Result = new BadRequestResult();
            return;
        }

        var agencyId = agencyIdObj != null ? (int)agencyIdObj : (int)travelAgencyObjectId;
        var userId = GetCurrentUserId();

        if (userId == 0)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var userExistsInAgency = await IsUserInAgency(agencyId, userId);
        Console.WriteLine(userExistsInAgency);

        if (!userExistsInAgency)
        {
            context.Result = new UnauthorizedResult();
            return;
        }


        await base.OnActionExecutionAsync(context, next);
    }

    private async Task<bool> IsUserInAgency(int agencyId, int userId)
    {
        var agency = await _agencyService.GetByIdAsync(agencyId);
        return agency.Users.Any(u => u.Id == userId);
    }

    private int GetCurrentUserId()
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userId, out var id))
        {
            return id;
        }

        return 0;
    }
}