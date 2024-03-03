namespace Book.App.Models;


public class AddTourModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxUsers { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Waypoints { get; set; } = string.Empty;
    public List<IFormFile> Images { get; set; } = new();
}
