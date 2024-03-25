using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class ReservationRepository : Repository<ReservationModel>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<List<ReservationModel>> GetAllReservations(int userId)
    {
        return await _dbContext.Reservations.Include(r => r.Tour).Include(r => r.User).Where(r => r.User.Id == userId).ToListAsync();
    }

    public Task<ReservationModel?> GetClosestReservation(int userId)
    {
        return GetUserReservationsQueryable(userId)
                        .OrderBy(r => r.Tour.StartDate)
                        .ThenBy(r => r.Tour.EndDate)
                        .Where(r => r.Tour.StartDate >= DateTime.Now || r.Tour.EndDate >= DateTime.Now)
                        .FirstOrDefaultAsync();
    }

    public async Task<ReservationModel?> GetReservationByTourAndUserId(int tourId, int userId)
    {
        return await _dbContext.Reservations.Include(r => r.User).Include(r => r.Tour).FirstOrDefaultAsync(r => r.User.Id == userId && r.Tour.Id == tourId);
    }

    public async Task<List<ReservationModel>> GetReservationsHistoryByUserId(int userId)
    {
        return await GetUserReservationsQueryable(userId)
                        .Where(r => r.Tour.EndDate < DateTime.Now)
                        .OrderByDescending(r => r.Tour.StartDate)
                        .ToListAsync();
    }

    public async Task<List<ReservationModel>> GetUserActiveAndFutureReservations(int userId)
    {
        return await GetUserReservationsQueryable(userId)
                        .Where(r => r.Tour.StartDate >= DateTime.Now || r.Tour.EndDate >= DateTime.Now)
                        .OrderBy(r => r.Tour.StartDate)
                        .ToListAsync();
    }

    public IQueryable<ReservationModel> GetUserReservationsQueryable(int userId)
    {
        return _dbContext.Reservations.Include(r => r.User)
                                    .Include(r => r.Tour).ThenInclude(t => t.Images)
                                    .Include(r => r.Tour).ThenInclude(t => t.Waypoints).ThenInclude(w => w.Images)
                                    .Where(r => r.User.Id == userId).AsQueryable();
    }

    public async Task<ReservationModel?> GetReservationWithAllJoinsByTourAndUserId(int tourId, int userId)
    {
        return await GetUserReservationsQueryable(userId).FirstOrDefaultAsync(r => r.Tour.Id == tourId);
    }
}