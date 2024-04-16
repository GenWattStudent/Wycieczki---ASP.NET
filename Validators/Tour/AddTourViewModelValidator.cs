using System.Security.Claims;
using Book.App.Services;
using Book.App.ViewModels;
using FluentValidation;

namespace Book.App.Validators;

public class AddTourViewModelValidator : AbstractValidator<AddTourViewModel>
{
    private int currentUserId;
    private readonly IAgencyService _agencyService;

    public AddTourViewModelValidator(IHttpContextAccessor httpContextAccessor, IAgencyService agencyService)
    {
        _agencyService = agencyService;
        currentUserId = GetCurrentUserId(httpContextAccessor);

        const int maxDescriptionLength = 500;
        const int minDescriptionLength = 3;

        const int maxNameLength = 50;
        const int minNameLength = 3;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(maxNameLength)
            .WithMessage($"Name must be less than {maxNameLength} characters")
            .MinimumLength(minNameLength)
            .WithMessage($"Name must be at least {minNameLength} characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(maxDescriptionLength)
            .WithMessage($"Description must be less than {maxDescriptionLength} characters")
            .MinimumLength(minDescriptionLength)
            .WithMessage($"Description must be at least {minDescriptionLength} characters");

        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Price is required")
            .GreaterThan(0)
            .WithMessage("Price must be greater than 0");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required")
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be greater than start date");

        // RuleFor(x => x.TravelAgencyId)
        //     .MustAsync((x, token) => x != 0 ? IsUserInThisAgency(currentUserId, x, token) : Task.FromResult(true))
        //     .WithMessage("You are not in this agency");

        RuleFor(x => x.MaxUsers)
            .NotEmpty()
            .WithMessage("Max users is required")
            .GreaterThan(0)
            .WithMessage("Max users must be greater than 0");
    }

    private int GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        return Convert.ToInt32(httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }

    private async Task<bool> IsUserInThisAgency(int userId, int agencyId, CancellationToken token)
    {
        var agency = await _agencyService.GetByIdAsync(agencyId);
        return agency.Users.Any(u => u.Id == userId);
    }
}