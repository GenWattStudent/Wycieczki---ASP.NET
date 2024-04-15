using Book.App.Models;
using FluentValidation;

namespace Book.App.Validators;

public class ContactValidator : AbstractValidator<ContactModel>
{
    public ContactValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Email is not valid");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone is required");
    }
}