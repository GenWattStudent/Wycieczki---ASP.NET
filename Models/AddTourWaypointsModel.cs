namespace Book.App.Models;

public class AddTourWaypointsModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<IFormFile> Images { get; set; } = new List<IFormFile>();
    public double Lat { get; set; }
    public double Lng { get; set; }
    public bool IsRoad { get; set; }
    public int Id { get; set; }
    public WaypointType Type { get; set; }
}