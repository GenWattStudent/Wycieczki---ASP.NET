using System.ComponentModel.DataAnnotations;

namespace Book.App.ViewModels;

public class AddTourViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = @"{0:0\.00}", ApplyFormatInEditMode = true)]
    public decimal Price { get; set; }
    public int MaxUsers { get; set; }
    public DateTime StartDate { get; set; } = DateTime.Now;
    // [DateGreaterThan("StartDate", ErrorMessage = "End date must be greater than start date.")]
    public DateTime EndDate { get; set; } = DateTime.Now;
    public bool IsVisible { get; set; } = false;
    public int TravelAgencyId { get; set; }
}
