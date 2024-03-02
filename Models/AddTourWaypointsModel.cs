namespace Book.App.Models;

public class AddTourWaypointsModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Image { get; set; }
    public decimal Lat { get; set; }
    public decimal Lng { get; set; }

}