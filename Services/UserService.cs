using Book.App.Models;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Register(UserModel registerModel)
    {
        var PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password);

        registerModel.Password = PasswordHash;

        _context.Users.Add(registerModel);
        await _context.SaveChangesAsync();
    }

    public async Task<UserModel?> GetByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<UserModel?> Login(LoginModel loginModel)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == loginModel.Username);

        if (user == null)
        {
            return null;
        }

        if (BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
        {
            return user;
        }

        return null;
    }

    public async Task Delete(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        if (user == null)
        {
            return;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}