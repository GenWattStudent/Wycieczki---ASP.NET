using Book.App.Repositories.UnitOfWork;
using Book.App.Specifications;
using Book.App.ViewModels;
using FluentValidation;

namespace Book.App.Validators;

public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
    private readonly IUnitOfWork _unitOfWork;

    public LoginViewModelValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        const int minPasswordLength = 3;

        RuleFor(x => x.Username)
            .NotEmpty()
                .WithMessage("Username cant be empty.")
            .MustAsync(UsernameExists)
                .WithMessage("Username or password are incorrect.");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Password cant be empty.")
            .MinimumLength(minPasswordLength)
                .WithMessage($"Password must be at least {minPasswordLength} characters long.")
            .MustAsync((model, password, token) => PasswordMatches(model.Username, password, token))
                    .WithMessage("Username or password are incorrect.");
    }

    private async Task<bool> UsernameExists(string username, CancellationToken token)
    {
        return await _unitOfWork.userRepository.GetSingleBySpec(new UserSpecification(username)) != null;
    }

    private async Task<bool> PasswordMatches(string username, string password, CancellationToken token)
    {
        var user = await _unitOfWork.userRepository.GetSingleBySpec(new UserSpecification(username));
        return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
    }
}