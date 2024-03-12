using Book.App.Models;
using Book.App.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _userRepository;
    private readonly string _userImageFolder = "users";
    private readonly FileService _fileService;

    public UserService(ApplicationDbContext context, UserRepository userRepository, FileService fileService)
    {
        _context = context;
        _userRepository = userRepository;
        _fileService = fileService;
    }

    public async Task Register(RegisterModel registerModel)
    {
        var PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password);

        registerModel.Password = PasswordHash;
        Console.WriteLine(registerModel.Image + "asdasdd");
        var userModel = new UserModel(registerModel);

        if (registerModel.Image != null)
        {
            userModel.ImagePath = await _fileService.SaveFile(registerModel.Image, _userImageFolder);
        }

        await _userRepository.Add(userModel);
        await _userRepository.SaveAsync();
    }

    public async Task RegisterAdmin(UserModel userModel)
    {
        var PasswordHash = BCrypt.Net.BCrypt.HashPassword(userModel.Password);

        userModel.Password = PasswordHash;

        await _userRepository.Add(userModel);
        await _userRepository.SaveAsync();
    }

    public async Task<UserModel?> GetByUsername(string username)
    {
        return await _userRepository.GetByUsername(username);
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