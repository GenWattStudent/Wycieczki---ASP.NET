using Book.App.Models;
using Book.App.ViewModels;

namespace Book.App.Services;

public interface IReservationService
{
    Task<List<ReservationModel>> GetUserActiveAndFutureReservations(int userId);
    Task<List<ReservationModel>> GetReservationsHistoryByUserId(int userId);
    Task<ReservationModel?> GetClosestReservation(int userId);
    Task AddTourToUser(int userId, int tourId);
    Task Cancel(int userId, int tourId);
    Task<TourModel?> GetTourById(int userId, int tourId);
    BookViewModel GetBookViewModel(TourModel tour);
    Task Delete(int userId, int tourId);
}


