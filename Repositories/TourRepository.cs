using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class TourRepository : Repository<TourModel>, ITourRepository
{
    public TourRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<TourModel> GetActiveTours()
    {
        return _dbContext.Tours
                    .Include(t => t.Images)
                    .Include(t => t.Reservations).ThenInclude(r => r.User)
                    .Where(t => t.StartDate <= DateTime.Now && t.EndDate >= DateTime.Now);
    }

    public IQueryable<TourModel> GetTour(int id)
    {
        return _dbContext.Tours
                    .Include(t => t.Users)
                    .Include(t => t.Waypoints).ThenInclude(w => w.Images)
                    .Include(t => t.Reservations).ThenInclude(r => r.User).ThenInclude(u => u.Contact)
                    .Include(t => t.Images)
                    .Where(t => t.Id == id);
    }

    public IQueryable<TourModel> GetTours()
    {
        return _dbContext.Tours
                    .Include(t => t.Images)
                    .Include(t => t.Reservations).ThenInclude(r => r.User)
                    .Where(t => t.StartDate >= DateTime.Now);
    }

}