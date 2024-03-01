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


    public async Task<List<TourModel>> GetBooks(int id)
    {
        // User have tours, get user by id and return its tours
        var tour = await _context.Users
            .Include(u => u.Tours)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (tour == null)
        {
            return new List<TourModel>();
        }

        return tour.Tours;
    }

}