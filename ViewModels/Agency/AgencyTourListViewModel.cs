using Book.App.Models;

namespace Book.App.ViewModels;

public class AgencyTourListViewModel
{
    public List<TourModel> Tours { get; set; } = new();
    public int AgencyId { get; set; }
}