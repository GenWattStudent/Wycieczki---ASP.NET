using Book.App.Repositories.UnitOfWork;
using Book.App.Specifications;
using Book.App.ViewModels;
using FluentValidation;

namespace Book.App.Validators;

public class RegisterViewModelValidator : AbstractValidator<RegisterViewModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterViewModelValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        const int minUsernameLength = 3;
        const int maxUsernameLength = 50;

        const int minPasswordLength = 3;
        const int maxPasswordLength = 50;

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required")
            .Length(minUsernameLength, maxUsernameLength)
            .WithMessage($"Username must be between {minUsernameLength} and {maxUsernameLength} characters");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .Length(minPasswordLength, maxPasswordLength)
            .WithMessage($"Password must be between {minPasswordLength} and {maxPasswordLength} characters");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm Password is required")
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match");

        RuleFor(x => x.Contact).SetValidator(new ContactValidator());
        RuleFor(x => x.Address).SetValidator(new AddressValidator());

        RuleFor(x => x.Username)
            .MustAsync(BeUniqueUsername)
            .WithMessage("Username is already taken");
    }

    private async Task<bool> BeUniqueUsername(string username, CancellationToken token)
    {
        var user = await _unitOfWork.userRepository.GetSingleBySpec(new UserSpecification(username));
        return user == null;
    }
}