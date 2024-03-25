using Book.App.Models;

namespace Book.App.Repositories;

public interface IReservationRepository : IRepository<ReservationModel>
{
    IQueryable<ReservationModel> GetUserReservationsQueryable(int userId);
    Task<List<ReservationModel>> GetUserActiveAndFutureReservations(int userId);
    Task<List<ReservationModel>> GetReservationsHistoryByUserId(int userId);
    Task<ReservationModel?> GetClosestReservation(int userId);
    Task<List<ReservationModel>> GetAllReservations(int userId);
    Task<ReservationModel?> GetReservationByTourAndUserId(int tourId, int userId);
    Task<ReservationModel?> GetReservationWithAllJoinsByTourAndUserId(int tourId, int userId);
}
