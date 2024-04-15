using Book.App.Models;

namespace Book.App.Repositories;

public interface IAgencyRepository : IRepository<TravelAgencyModel>
{
    public Task<TravelAgencyModel?> GetByName(string name);
}