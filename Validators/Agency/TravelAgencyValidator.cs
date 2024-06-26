using System.Security.Claims;
using Book.App.Repositories.UnitOfWork;
using Book.App.ViewModels;
using FluentValidation;

namespace Book.App.Validators;

public class TravelAgencyValidator : AbstractValidator<CreateAgencyViewModel>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly int _currentUserId;

    public TravelAgencyValidator(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _currentUserId = GetCurrentUserId(httpContextAccessor);

        const int minNameLength = 3;
        const int maxNameLength = 50;

        const int minDescriptionLength = 3;
        const int maxDescriptionLength = 800;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(minNameLength, maxNameLength)
            .WithMessage($"Name must be between {minNameLength} and {maxNameLength} characters");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MustAsync((x, model, token) => IsUniqueName(x.Name, x.Id, token))
            .WithMessage("Name is already taken");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .Length(minDescriptionLength, maxDescriptionLength)
            .WithMessage($"Description must be between {minDescriptionLength} and {maxDescriptionLength} characters");

        RuleFor(x => x.Address).SetValidator(new AddressValidator());

        RuleFor(x => x.Id)
            .MustAsync((x, token) => IsUserHasAgency(_currentUserId, x, token))
            .WithMessage("User already has an agency");
    }

    private async Task<bool> IsUniqueName(string name, int id, CancellationToken token)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        var agency = await _unitOfWork.agencyRepository.GetByName(name);
        return agency == null || agency.Id == id;
    }

    private async Task<bool> IsUserHasAgency(int userId, int id, CancellationToken token)
    {
        if (id != 0)
        {
            return true;
        }

        var user = await _unitOfWork.userRepository.GetById(userId);
        Console.WriteLine(user?.TravelAgencyId);

        return user?.TravelAgencyId == null;
    }

    private int GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
    {
        // Get the current user ID from HttpContext or your authentication mechanism
        return Convert.ToInt32(httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    }
}