using Book.App.Filters;
using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class TourRepository : Repository<TourModel>, ITourRepository
{
    public TourRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<TourModel>> GetActiveTours()
    {
        return await _dbContext.Tours.
                        Include(t => t.Images)
                        .Include(t => t.Users)
                        .Where(t => t.StartDate <= DateTime.Now && t.EndDate >= DateTime.Now)
                        .ToListAsync();
    }

    public async Task<TourModel?> GetTour(int id)
    {
        return await _dbContext.Tours
                        .Include(t => t.Users)
                        .Include(t => t.Waypoints).ThenInclude(w => w.Images)
                        .Include(t => t.Reservations).ThenInclude(r => r.User).ThenInclude(u => u.Contact)
                        .Include(t => t.Images).FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<TourModel>> GetTours()
    {
        return await _dbContext.Tours.Include(t => t.Images)
                                    .Include(t => t.Reservations).ThenInclude(r => r.User)
                                    .Where(t => t.StartDate >= DateTime.Now).ToListAsync();
    }

    public async Task<List<TourModel>> GetTours(FilterModel filterModel)
    {
        var tours = _dbContext.Tours
            .Include(t => t.Images)
            .Include(t => t.Reservations).ThenInclude(r => r.User)
            .Where(t => t.StartDate >= DateTime.Now);
        var filters = new List<IFilter> { new SearchFilter(filterModel.SearchString), new PriceFilter(filterModel.MinPrice, filterModel.MaxPrice) };

        tours = filters.Aggregate(tours, (current, filter) => filter.Process(current));

        switch (filterModel.OrderBy)
        {
            case OrderBy.Date:
                tours = filterModel.OrderDirection == OrderDirection.Asc ? tours.OrderBy(t => t.StartDate) : tours.OrderByDescending(t => t.StartDate);
                break;
            case OrderBy.Price:
                tours = filterModel.OrderDirection == OrderDirection.Asc ? tours.OrderBy(t => t.Price) : tours.OrderByDescending(t => t.Price);
                break;
        }

        return await tours.ToListAsync();
    }
}