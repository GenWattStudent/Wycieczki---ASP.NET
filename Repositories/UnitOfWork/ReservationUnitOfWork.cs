namespace Book.App.Repositories.UnitOfWork;

public class ReservationUnitOfWork
{
    public readonly IReservationRepository reservationRepository;
    public readonly ITourRepository tourRepository;
    public readonly IUserRepository userRepository;
    private readonly ApplicationDbContext _dbContext;

    public ReservationUnitOfWork(IReservationRepository reservationRepository, ITourRepository tourRepository, IUserRepository userRepository, ApplicationDbContext dbContext)
    {
        this.reservationRepository = reservationRepository;
        this.tourRepository = tourRepository;
        this.userRepository = userRepository;
        _dbContext = dbContext;
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}