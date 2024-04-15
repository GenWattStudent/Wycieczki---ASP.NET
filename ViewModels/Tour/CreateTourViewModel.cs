using Book.App.Models;

namespace Book.App.ViewModels;

public class CreateTourViewModel
{
    public TravelAgencyModel AgencyModel { get; set; } = new();
    public TourModel TourModel { get; set; } = new();
    public AddTourViewModel AddTourViewModel { get; set; } = new();
}