using Book.App.Models;
using Book.App.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class BookService
{
    private readonly ApplicationDbContext _context;
    private readonly GeoService _geoService;

    public BookService(ApplicationDbContext context, GeoService geoService)
    {
        _context = context;
        _geoService = geoService;
    }

    public IQueryable<List<TourModel>> GetToursByUserIdQueryable(int userId)
    {
        return _context.Users
            .Include(u => u.Tours)
            .ThenInclude(t => t.Images)
            .Include(u => u.Tours)
            .ThenInclude(t => t.Waypoints)
            .Where(u => u.Id == userId).Select(u => u.Tours);
    }

    public async Task<List<TourModel>> GetActiveAndFutureToursByUserId(int userId)
    {
        var userWithTours = GetToursByUserIdQueryable(userId);

        return await userWithTours.SelectMany(t => t)
                                  .Where(t => t.StartDate >= DateTime.Now || t.EndDate >= DateTime.Now)
                                  .OrderBy(t => t.StartDate)
                                  .ToListAsync();
    }

    public async Task<List<TourModel>> GetToursHistoryByUserId(int userId)
    {
        var userWithTours = GetToursByUserIdQueryable(userId);

        return await userWithTours.SelectMany(t => t)
                                  .Where(t => t.EndDate < DateTime.Now)
                                  .OrderByDescending(t => t.StartDate)
                                  .ToListAsync();
    }

    public async Task<TourModel?> GetClosestTourByUserId(int userId)
    {
        var userWithTours = GetToursByUserIdQueryable(userId);
        return await userWithTours.SelectMany(t => t)
                                  .Where(t => t.StartDate >= DateTime.Now || t.EndDate >= DateTime.Now)
                                  .OrderBy(t => t.StartDate)
                                  .FirstOrDefaultAsync();
    }

    public async Task AddTourToUser(int userId, int tourId)
    {
        var user = await _context.Users.Include(u => u.Tours).FirstOrDefaultAsync(u => u.Id == userId);
        var tour = await _context.Tours.Include(t => t.Users).FirstOrDefaultAsync(t => t.Id == tourId);

        if (user == null || tour == null)
        {
            throw new Exception("User or tour not found");
        }

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

        if (user.Tours.Any(t => t.StartDate <= tour.StartDate && t.EndDate >= tour.EndDate || t.StartDate >= tour.StartDate && t.EndDate <= tour.EndDate))
        {
            throw new Exception("User already booked a tour in this time");
        }

        user.Tours.Add(tour);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCancelTour(int userId, int tourId)
    {
        var user = await _context.Users.Include(u => u.Tours).FirstOrDefaultAsync(u => u.Id == userId);
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == tourId);

        if (tour.StartDate < DateTime.Now && tour.EndDate > DateTime.Now)
        {
            throw new Exception("Tour already started");
        }

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
        var tour = await _context.Tours.Include(t => t.Users).Include(t => t.Waypoints).ThenInclude(w => w.Images).Include(t => t.Images).FirstOrDefaultAsync(t => t.Id == tourId && t.Users.Any(u => u.Id == userId));

        return tour;
    }

    public BookViewModel GetBookViewModel(TourModel tour)
    {
        var bookViewModel = new BookViewModel { TourModel = tour, Distance = _geoService.CalculateDistance(tour.Waypoints) };

        // the trip started and not ended calculate dsitance and which waypoint you are
        if (tour.StartDate <= DateTime.Now && tour.EndDate >= DateTime.Now)
        {
            bookViewModel.PercentOfTime = (DateTime.Now - tour.StartDate).TotalDays / (tour.EndDate - tour.StartDate).TotalDays;
            var nextWaypointData = _geoService.CalculateDistanceToNextWaypoint(tour.Waypoints, bookViewModel.PercentOfTime);
            bookViewModel.NextWaypointData = nextWaypointData;
        }
        else if (tour.StartDate > DateTime.Now)
        {
            bookViewModel.PercentOfTime = 0;
        }
        else
        {
            bookViewModel.PercentOfTime = 1;
        }

        return bookViewModel;
    }
}