using Book.App.Models;

namespace Book.App.Repositories;

public class AgencyRepository : Repository<TravelAgencyModel>, IAgencyRepository
{
    public AgencyRepository(ApplicationDbContext context) : base(context)
    {
    }
}