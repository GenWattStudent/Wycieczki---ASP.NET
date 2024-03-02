using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class WaypointModel
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public int TourId { get; set; }
    public TourModel Tour { get; set; } = new();
}