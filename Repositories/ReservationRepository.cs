using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class ReservationRepository : Repository<ReservationModel>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<ReservationModel> GetAllReservations(int userId)
    {
        return _dbContext.Reservations
                        .Include(r => r.Tour)
                        .Include(r => r.User)
                        .Where(r => r.User.Id == userId);
    }

    public IQueryable<ReservationModel> GetReservationByTourAndUserId()
    {
        return _dbContext.Reservations
                        .Include(r => r.User)
                        .Include(r => r.Tour);
    }

    public IQueryable<ReservationModel> GetReservationsHistoryByUserId(int userId)
    {
        return GetUserReservationsQueryable(userId)
                        .Where(r => r.Tour.EndDate < DateTime.Now)
                        .OrderByDescending(r => r.Tour.StartDate);
    }

    public IQueryable<ReservationModel> GetUserActiveAndFutureReservations(int userId)
    {
        return GetUserReservationsQueryable(userId)
                        .Where(r => r.Tour.StartDate >= DateTime.Now || r.Tour.EndDate >= DateTime.Now)
                        .OrderBy(r => r.Tour.StartDate);
    }

    public IQueryable<ReservationModel> GetUserReservationsQueryable(int userId)
    {
        return _dbContext.Reservations
                        .Include(r => r.User)
                        .Include(r => r.Tour).ThenInclude(t => t.Images)
                        .Include(r => r.Tour).ThenInclude(t => t.Waypoints).ThenInclude(w => w.Images)
                        .Where(r => r.User.Id == userId);
    }
}