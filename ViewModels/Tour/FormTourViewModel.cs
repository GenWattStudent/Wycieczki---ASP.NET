using Book.App.Models;

namespace Book.App.ViewModels;

public class FormTourViewModel
{
    public AddTourViewModel AddTourViewModel { get; set; } = new();
    public TourModel? TourModel { get; set; }
    public int? AgencyId { get; set; }
}