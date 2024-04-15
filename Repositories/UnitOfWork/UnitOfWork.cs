namespace Book.App.Repositories.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public IUserRepository userRepository => new UserRepository(_dbContext);
    public ITourRepository tourRepository => new TourRepository(_dbContext);
    public IReservationRepository reservationRepository => new ReservationRepository(_dbContext);
    public IImageRepository imageRepository => new ImageRepository(_dbContext);
    public IMealRepository mealRepository => new MealRepository(_dbContext);
    public IWaypointRepository waypointRepository => new WaypointRepository(_dbContext);
    public IAgencyRepository agencyRepository => new AgencyRepository(_dbContext);

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