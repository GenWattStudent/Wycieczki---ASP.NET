
using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class UserRepository : Repository<UserModel>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public IQueryable<UserModel> GetByUsername(string username)
    {
        return _dbContext.Users
                .Include(u => u.Contact)
                .Include(u => u.Address)
                .Include(u => u.Preferences);
    }
}
