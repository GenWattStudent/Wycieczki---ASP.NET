using Book.App.Models;

namespace Book.App.Repositories;

public interface IReservationRepository : IRepository<ReservationModel>
{
    IQueryable<ReservationModel> GetUserReservationsQueryable(int userId);
    IQueryable<ReservationModel> GetUserActiveAndFutureReservations(int userId);
    IQueryable<ReservationModel> GetReservationsHistoryByUserId(int userId);
    IQueryable<ReservationModel> GetAllReservations(int userId);
    IQueryable<ReservationModel> GetReservationByTourAndUserId();
}
