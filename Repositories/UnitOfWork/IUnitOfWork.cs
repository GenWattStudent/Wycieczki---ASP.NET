namespace Book.App.Repositories.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IReservationRepository reservationRepository { get; }
    ITourRepository tourRepository { get; }
    IUserRepository userRepository { get; }
    IImageRepository imageRepository { get; }
    IMealRepository mealRepository { get; }
    IWaypointRepository waypointRepository { get; }
    IAgencyRepository agencyRepository { get; }
    Task SaveAsync();
}