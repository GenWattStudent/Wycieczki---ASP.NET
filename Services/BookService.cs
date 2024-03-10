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

        user.Tours.Add(tour);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCancelTour(int userId, int tourId)
    {
        var user = await _context.Users.Include(u => u.Tours).FirstOrDefaultAsync(u => u.Id == userId);
        var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == tourId);

        if (tour.StartDate < DateTime.Now)
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
}