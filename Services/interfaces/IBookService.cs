using Book.App.Models;
using Book.App.ViewModels;

namespace Book.App.Services;

public interface IBookService
{
    Task<List<ReservationModel>> GetUserActiveAndFutureReservations(int userId);
    Task<List<ReservationModel>> GetReservationsHistoryByUserId(int userId);
    Task<ReservationModel?> GetClosestReservation(int userId);
    Task AddTourToUser(int userId, int tourId);
    Task DeleteCancelTour(int userId, int tourId);
    Task<TourModel?> GetTourById(int userId, int tourId);
    BookViewModel GetBookViewModel(TourModel tour);
    Task DeleteReservation(int userId, int tourId);
}


