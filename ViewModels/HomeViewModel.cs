using Book.App.Models;

namespace Book.App.ViewModels;

public class HomeViewModel
{
    public BookViewModel? Book { get; set; }
    public TourModel? Tour { get; set; }
}