namespace Book.App.ViewModels;

public class EditTourViewModel : AddTourViewModel
{
    public int Id { get; set; }
    public List<AddTourWaypointsViewModel> Waypoints { get; set; } = new();
    public List<IFormFile> Images { get; set; } = new();
}