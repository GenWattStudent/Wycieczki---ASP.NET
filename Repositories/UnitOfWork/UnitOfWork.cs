namespace Book.App.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public IUserRepository userRepository => new UserRepository(_dbContext);
    public ITourRepository tourRepository => new TourRepository(_dbContext);
    public IReservationRepository reservationRepository => new ReservationRepository(_dbContext);

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public UnitOfWork()
    {
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}