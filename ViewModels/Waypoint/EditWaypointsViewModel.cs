using Book.App.Models;

namespace Book.App.ViewModels;

public class EditWaypointsViewModel
{
    public List<WaypointModel> Waypoints = new();
    public int TourId { get; set; }
    public WaypointModel? CurrentWaypoint { get; set; }
    public TourModel? TourModel { get; set; }
}