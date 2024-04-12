using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class AgencyRepository : Repository<TravelAgencyModel>, IAgencyRepository
{
    public AgencyRepository(ApplicationDbContext context) : base(context)
    {

    }

    public async Task<TravelAgencyModel?> GetByName(string name)
    {
        return await _dbContext.TravelAgencies.FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower());
    }
}