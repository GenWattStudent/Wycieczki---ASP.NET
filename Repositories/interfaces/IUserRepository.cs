using Book.App.Models;

namespace Book.App.Repositories;

public interface IUserRepository : IRepository<UserModel>
{
    Task<UserModel?> GetByUsername(string username);
}