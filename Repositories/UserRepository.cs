
using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Repositories;

public class UserRepository : Repository<UserModel>
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<UserModel?> GetByUsername(string username)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);
    }
}
