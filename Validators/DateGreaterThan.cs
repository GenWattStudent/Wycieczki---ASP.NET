using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Book.App.Validators;

public class DateGreaterThanAttribute : ValidationAttribute, IClientModelValidator
{
    private readonly string comparisonProperty;

    public DateGreaterThanAttribute(string comparisonProperty)
    {
        this.comparisonProperty = comparisonProperty;
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-dategreaterthan", ErrorMessage ?? GetErrorMessage(context.ModelMetadata.GetDisplayName()));
        context.Attributes.Add("data-val-dategreaterthan-propertyname", comparisonProperty);
        var currentProperty = context.ModelMetadata.PropertyName;
        context.Attributes.Add("data-val-dategreaterthan-target", currentProperty);
    }

    private string GetErrorMessage(string displayName)
    {
        return $"{displayName} must be greater than the target date.";
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        DateTime dateTime = (DateTime)value;
        DateTime target = (DateTime)validationContext.ObjectType.GetProperty(comparisonProperty).GetValue(validationContext.ObjectInstance, null);
        Console.WriteLine(dateTime + " dupa " + target);
        if (dateTime > target)
        {
            return ValidationResult.Success;
        }
        else
        {
            return new ValidationResult("Date is not greater than the target date.");
        }
    }
}