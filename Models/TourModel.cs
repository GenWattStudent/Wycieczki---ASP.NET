using System.ComponentModel.DataAnnotations;

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
    public string ImageUrl { get; set; } = string.Empty;
    [Required]
    [Range(1, 10000)]
    public decimal Price { get; set; }
}