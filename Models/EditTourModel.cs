namespace Book.App.Models;

public class EditTourModel : AddTourModel
{
    public int Id { get; set; }
    public List<AddTourWaypointsModel> Waypoints { get; set; } = new();
    public List<IFormFile> Images { get; set; } = new();
}