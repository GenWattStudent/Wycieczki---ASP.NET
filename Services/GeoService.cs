using Book.App.Models;

namespace Book.App.Services;

public class NextWaypointData
{
    public float Distance;
    public double TourLat;
    public double TourLon;
    public WaypointModel NextWaypoint;
}

public class GeoService
{
    public float CalculateDistance(List<WaypointModel> waypoints)
    {
        float distance = 0;

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            var point1 = waypoints[i];
            var point2 = waypoints[i + 1];

            distance += CalculateDistance(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude);
        }

        return distance;
    }

    private float CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the earth in km
        var dLat = (lat2 - lat1) * Math.PI / 180;  // deg2rad below
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var d = R * c; // Distance in km
        return (float)d;
    }

    public NextWaypointData CalculateDistanceToNextWaypoint(List<WaypointModel> waypoints, double percentOfTime)
    {
        var distance = 0f;
        var tourLat = 0d;
        var tourLon = 0d;
        var nextWaypoint = new WaypointModel();

        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            var point1 = waypoints[i];
            var point2 = waypoints[i + 1];

            var distanceBetweenPoints = CalculateDistance(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude);

            if (percentOfTime * distanceBetweenPoints > distance)
            {
                distance = distanceBetweenPoints;
                // Interpolate the current position based on percentOfTime
                tourLat = point1.Latitude + percentOfTime * (point2.Latitude - point1.Latitude);
                tourLon = point1.Longitude + percentOfTime * (point2.Longitude - point1.Longitude);
                nextWaypoint = point2;
                break;
            }
            else
            {
                distance -= (float)(percentOfTime * distanceBetweenPoints);
            }
        }

        return new NextWaypointData
        {
            Distance = distance,
            TourLat = tourLat,
            TourLon = tourLon,
            NextWaypoint = nextWaypoint,
        };
    }
}