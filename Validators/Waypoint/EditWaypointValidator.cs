using Book.App.Models;
using FluentValidation;

namespace Book.App.Validators;

public class EditWaypointValidator : AbstractValidator<WaypointModel>
{
    public EditWaypointValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Description).MaximumLength(500).WithMessage("Description is too long");
    }
}