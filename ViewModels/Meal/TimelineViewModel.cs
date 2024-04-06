using Book.App.Models;

namespace Book.App.ViewModels;

public class TimelineViewModel
{
    public List<MealModel> Meals = new();
    public bool isEditMode = false;
    public int TourId { get; set; }
}