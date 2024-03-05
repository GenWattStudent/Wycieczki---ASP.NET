using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class WaypointModel
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ImageModel> Images { get; set; } = new List<ImageModel>();
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int TourId { get; set; }
    public TourModel Tour { get; set; } = new();
    public bool IsRoad { get; set; }
}