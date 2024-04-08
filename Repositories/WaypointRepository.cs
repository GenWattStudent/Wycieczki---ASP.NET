using Book.App.Models;

namespace Book.App.Repositories;

public class WaypointRepository : Repository<WaypointModel>, IWaypointRepository
{
    public WaypointRepository(ApplicationDbContext context) : base(context)
    {
    }
}