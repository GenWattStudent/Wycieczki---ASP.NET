using Book.App.ViewModels;
using FluentValidation;

namespace Book.App.Validators;

public class TravelAgencyValidator : AbstractValidator<CreateAgencyViewModel>
{

    public TravelAgencyValidator()
    {
        const int minNameLength = 3;
        const int maxNameLength = 50;

        const int minDescriptionLength = 3;
        const int maxDescriptionLength = 800;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .Length(minNameLength, maxNameLength)
            .WithMessage($"Name must be between {minNameLength} and {maxNameLength} characters");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .Length(minDescriptionLength, maxDescriptionLength)
            .WithMessage($"Description must be between {minDescriptionLength} and {maxDescriptionLength} characters");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required");
    }
}