using Book.App.Models;

namespace Book.App.ViewModels;

public class AddTourWaypointsViewModel
{
    private string _description = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Description
    {
        get => _description;
        set => _description = value ?? string.Empty; // Ensures never set to null
    }
    public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    public double Lat { get; set; }
    public double Lng { get; set; }
    public bool IsRoad { get; set; }
    public int Id { get; set; }
    public WaypointType Type { get; set; }

}