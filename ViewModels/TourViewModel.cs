using Book.App.Models;

namespace Book.App.ViewModels;

public class TourViewModel
{
    public TourModel TourModel;
    public float Distance;
    public double PercentOfTime;
    public float DistanceToNextWaypoint;
    public double TourLat;
    public double TourLon;
    public WaypointModel NextWaypoint;
}

