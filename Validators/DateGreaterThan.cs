using System.ComponentModel.DataAnnotations;

namespace Book.App.Validators;

public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateGreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var currentValue = (DateTime)value;

        var comparisonValue = (DateTime)validationContext.ObjectType.GetProperty(_comparisonProperty).GetValue(validationContext.ObjectInstance);
        Console.WriteLine("Current value: " + currentValue + " Comparison value: " + comparisonValue);
        if (currentValue <= comparisonValue)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}