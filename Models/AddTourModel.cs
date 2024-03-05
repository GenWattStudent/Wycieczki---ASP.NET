using System.ComponentModel.DataAnnotations;

namespace Book.App.Models;

public class AddTourModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    [DataType(DataType.Currency)]
    [DisplayFormat(DataFormatString = @"{0:0\.00}", ApplyFormatInEditMode = true)]
    public decimal Price { get; set; }
    public int MaxUsers { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<AddTourWaypointsModel> Waypoints { get; set; } = new();
    public List<IFormFile> Images { get; set; } = new();
}
