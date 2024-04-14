using Book.App.Models;
using FluentValidation;

namespace Book.App.Validators;

public class AddressValidator : AbstractValidator<AddressModel>
{
    public AddressValidator()
    {
        const int minStreetLength = 3;
        const int maxStreetLength = 50;

        const int minCityLength = 3;
        const int maxCityLength = 50;

        const int minZipLength = 3;
        const int maxZipLength = 10;

        const int minCountryLength = 3;
        const int maxCountryLength = 50;

        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street is required")
            .Length(minStreetLength, maxStreetLength)
            .WithMessage($"Street must be between {minStreetLength} and {maxStreetLength} characters");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .Length(minCityLength, maxCityLength)
            .WithMessage($"City must be between {minCityLength} and {maxCityLength} characters");

        RuleFor(x => x.Zip)
            .NotEmpty()
            .WithMessage("Zip is required")
            .Length(minZipLength, maxZipLength)
            .WithMessage($"Zip must be between {minZipLength} and {maxZipLength} characters");

        RuleFor(x => x.Country)
            .NotEmpty()
            .WithMessage("Country is required")
            .Length(minCountryLength, maxCountryLength)
            .WithMessage($"Country must be between {minCountryLength} and {maxCountryLength} characters");
    }
}