using System.ComponentModel.DataAnnotations;
using Book.App.Validators;

namespace Book.App.ViewModels;

public class AddTourViewModel
{
    [Required]
    [StringLength(100)]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [StringLength(500)]
    [MinLength(3)]
    public string Description { get; set; } = string.Empty;
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = @"{0:0\.00}", ApplyFormatInEditMode = true)]
    [Range(1, 10000)]
    public decimal Price { get; set; }
    [Range(1, 100)]
    public int MaxUsers { get; set; }
    public DateTime StartDate { get; set; }
    [DateGreaterThan("StartDate", ErrorMessage = "End date must be greater than start date.")]
    public DateTime EndDate { get; set; }
}
