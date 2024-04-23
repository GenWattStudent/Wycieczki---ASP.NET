using Book.App.Models;

namespace Book.App.ViewModels;

public class TourListViewModel
{
    public List<TourModel> Tours { get; set; } = new();
    public FilterModel FilterModel { get; set; } = new();
    public int TotalItems { get; set; }
}