using Book.App.ViewModels;
using FluentValidation;

namespace Book.App.Validators;

public class EditUserViewModelValidator : AbstractValidator<EditUserViewModel>
{
    public EditUserViewModelValidator()
    {
        RuleFor(x => x.Contact).SetValidator(new ContactValidator());
        RuleFor(x => x.Address).SetValidator(new AddressValidator());
    }
}