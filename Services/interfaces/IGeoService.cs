using Book.App.Models;

namespace Book.App.Services;

public interface IGeoService
{

    float CalculateDistance(List<WaypointModel> waypoints);
    NextWaypointData CalculateDistanceToNextWaypoint(List<WaypointModel> waypoints, double percentOfTime);

}