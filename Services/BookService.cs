using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class BookService
{
    private readonly ApplicationDbContext _context;

    public BookService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TourModel>> GetToursByUserId(int userId)
    {
        var userWithTours = await _context.Users.Include(u => u.Tours).ThenInclude(t => t.Images).FirstOrDefaultAsync(u => u.Id == userId);
        return userWithTours?.Tours.ToList() ?? new();
    }

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

    public async Task AddTourToUser(int userId, int tourId)
    {
        var user = await _context.Users.Include(u => u.Tours).FirstOrDefaultAsync(u => u.Id == userId);
        var tour = await _context.Tours.Include(t => t.Users).FirstOrDefaultAsync(t => t.Id == tourId);

        if (user == null || tour == null)
        {
            throw new Exception("User or tour not found");
        }

        // )if user already book this tour
        if (user.Tours.Any(t => t.Id == tourId))
        {
            throw new Exception("User already booked this tour");
        }

        if (tour.StartDate < DateTime.Now)
        {
            throw new Exception("Tour already started");
        }

        if (tour.MaxUsers <= tour.Users.Count)
        {
            throw new Exception("Tour is full");
        }

        user.Tours.Add(tour);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCancelTour(int userId, int tourId)
    {
        var user = await _context.Users.Include(u => u.Tours).FirstOrDefaultAsync(u => u.Id == userId);
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == tourId);

        if (user != null && tour != null)
        {
            user.Tours.Remove(tour);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("User or tour not found");
        }
    }

    public async Task<TourModel?> GetTourById(int userId, int tourId)
    {
        var tour = await _context.Tours.Include(t => t.Users).Include(t => t.Waypoints).Include(t => t.Images).FirstOrDefaultAsync(t => t.Id == tourId && t.Users.Any(u => u.Id == userId));

        return tour;
    }
}