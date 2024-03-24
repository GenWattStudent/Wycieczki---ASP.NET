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

    public IQueryable<ReservationModel> GetUserReservationsQueryable(int userId)
    {
        return _context.Reservations.Include(r => r.User)
                                    .Include(r => r.Tour).ThenInclude(t => t.Images)
                                    .Include(r => r.Tour).ThenInclude(t => t.Waypoints).ThenInclude(w => w.Images)
                                    .Where(r => r.User.Id == userId).AsQueryable();
    }

    public async Task<List<ReservationModel>> GetUserActiveAndFutureReservations(int userId)
    {
        var reservations = GetUserReservationsQueryable(userId);

        return await reservations.Where(r => r.Tour.StartDate >= DateTime.Now || r.Tour.EndDate >= DateTime.Now).OrderBy(r => r.Tour.StartDate)
                                  .ToListAsync();
    }

    public async Task<List<ReservationModel>> GetReservationsHistoryByUserId(int userId)
    {
        var reservations = GetUserReservationsQueryable(userId);

        return await reservations.Where(r => r.Tour.EndDate < DateTime.Now).OrderByDescending(r => r.Tour.StartDate).ToListAsync();
    }

    public async Task<ReservationModel?> GetClosestReservation(int userId)
    {
        var reservations = GetUserReservationsQueryable(userId);

        return await reservations.OrderBy(r => r.Tour.StartDate)
                                  .ThenBy(r => r.Tour.EndDate)
                                  .Where(r => r.Tour.StartDate >= DateTime.Now || r.Tour.EndDate >= DateTime.Now)
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

        if (user.Tours.Any(t =>
            t.StartDate <= tour.StartDate && t.EndDate >= tour.EndDate
        || t.StartDate >= tour.StartDate && t.EndDate <= tour.EndDate
        || t.StartDate <= tour.StartDate && t.EndDate >= tour.StartDate
        || t.StartDate <= tour.EndDate && t.EndDate >= tour.EndDate))
        {
            throw new Exception("User already booked a tour in this time");
        }

        var reservation = new ReservationModel
        {
            User = user,
            Tour = tour
        };

        user.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCancelTour(int userId, int tourId)
    {
        var reservation = await _context.Reservations.Include(r => r.User).Include(r => r.Tour).FirstOrDefaultAsync(r => r.User.Id == userId && r.Tour.Id == tourId);

        if (reservation == null)
        {
            throw new Exception("Reservation not found");
        }

        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();

        if (reservation.Tour.StartDate < DateTime.Now && reservation.Tour.EndDate > DateTime.Now)
        {
            throw new Exception("Tour already started");
        }
    }

    public async Task<TourModel?> GetTourById(int userId, int tourId)
    {
        var reservation = await GetUserReservationsQueryable(userId).FirstOrDefaultAsync(r => r.Tour.Id == tourId);

        return reservation != null ? reservation.Tour : null;
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

    public async Task DeleteReservation(int userId, int tourId)
    {
        var reservation = await _context.Reservations.Include(r => r.User).Include(r => r.Tour).FirstOrDefaultAsync(r => r.User.Id == userId && r.Tour.Id == tourId);

        if (reservation != null)
        {
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new Exception("User or tour not found");
        }
    }
}