using Book.App.Models;

namespace Book.App.Repositories.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IReservationRepository reservationRepository { get; }
    ITourRepository tourRepository { get; }
    IUserRepository userRepository { get; }
    Task SaveAsync();
}