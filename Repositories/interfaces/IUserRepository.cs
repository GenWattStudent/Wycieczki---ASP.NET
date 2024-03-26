using Book.App.Models;

namespace Book.App.Repositories;

public interface IUserRepository : IRepository<UserModel>
{
    IQueryable<UserModel> GetByUsername(string username);
}