using System.ComponentModel.DataAnnotations;
using Book.App.Validators;

namespace Book.App.Models;

public class TourModel
{
    [Key]
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [StringLength(500)]
    [MinLength(3)]
    public string Description { get; set; } = string.Empty;
    public List<ImageModel> Images { get; set; } = new();
    [Required]
    [Range(1, 10000)]
    public decimal Price { get; set; }
    public List<UserModel> Users { get; set; } = new();
    [Required]
    [Range(1, 100)]
    public int MaxUsers { get; set; }
    public DateTime StartDate { get; set; }
    [DateGreaterThan("StartDate", ErrorMessage = "End date must be greater than start date.")]
    public DateTime EndDate { get; set; }
    public List<WaypointModel> Waypoints { get; set; } = new();
}