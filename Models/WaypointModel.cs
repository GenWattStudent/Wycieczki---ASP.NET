using Book.App.ViewModels;

namespace Book.App.Models;

public enum WaypointType
{
    Start,
    Marker,
    End,
    Road
}

public class WaypointModel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ImageModel> Images { get; set; } = new List<ImageModel>();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int TourId { get; set; }
    public TourModel Tour { get; set; } = new();
    public bool IsRoad { get; set; }
    public WaypointType Type { get; set; }

    public WaypointModel()
    {
    }

    public WaypointModel(AddTourWaypointsViewModel waypoint)
    {
        Name = waypoint.Name;
        Description = waypoint.Description;
        Latitude = waypoint.Lat;
        Longitude = waypoint.Lng;
        IsRoad = waypoint.IsRoad;
        Type = waypoint.Type;
    }
}