using Book.App.Models;
using Book.App.Repositories;
using Book.App.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Book.App.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly string _userImageFolder = "users";
    private readonly IFileService _fileService;
    private readonly IAgencyService _agencyService;

    public UserService(IUserRepository userRepository, IFileService fileService, IAgencyService agencyService)
    {
        _userRepository = userRepository;
        _fileService = fileService;
        _agencyService = agencyService;
    }

    public async Task Register(RegisterModel registerModel)
    {
        var PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password);

        registerModel.Password = PasswordHash;

        var userModel = new UserModel(registerModel);

        if (registerModel.Image != null)
        {
            userModel.ImagePath = await _fileService.SaveFile(registerModel.Image, _userImageFolder);
        }

        _userRepository.Add(userModel);
        await _userRepository.SaveAsync();
    }

    public async Task EditUserInfo(RegisterModel userViewModel, UserModel userModel)
    {
        if (userModel == null)
        {
            return;
        }

        if (userModel.Contact == null) userModel.Contact = new ContactModel();
        if (userModel.Address == null) userModel.Address = new AddressModel();

        userModel.Contact.SetContact(userViewModel.Contact);
        userModel.Address.SetAddress(userViewModel.Address);

        if (userViewModel.Image != null)
        {
            if (userModel.ImagePath != null) await _fileService.DeleteFile(userModel.ImagePath);
            userModel.ImagePath = await _fileService.SaveFile(userViewModel.Image, _userImageFolder);
        }

        await _userRepository.SaveAsync();
    }

    public async Task RegisterAdmin(UserModel userModel)
    {
        var PasswordHash = BCrypt.Net.BCrypt.HashPassword(userModel.Password);

        userModel.Password = PasswordHash;

        _userRepository.Add(userModel);
        await _userRepository.SaveAsync();
    }

    public async Task<UserModel?> GetByUsername(string username)
    {
        return await _userRepository.GetByUsername(username).Include(u => u.TravelAgency).Include(u => u.Roles).FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());
    }

    public async Task<UserModel?> Login(LoginViewModel loginModel)
    {
        var user = await _userRepository.GetByUsername(loginModel.Username).Include(u => u.Roles).FirstOrDefaultAsync(x => x.Username == loginModel.Username);

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
        var user = await _userRepository.GetByUsername(username).Include(u => u.TravelAgency).FirstOrDefaultAsync(x => x.Username == username);
        if (user == null)
        {
            return;
        }

        if (user.TravelAgency != null)
        {
            await _agencyService.LeaveAsync(user.Id);
        }

        await _userRepository.Remove(user.Id);
        if (user.ImagePath != null) await _fileService.DeleteFile(user.ImagePath);
        await _userRepository.SaveAsync();
    }

    public async Task DeleteImage(string? username)
    {
        var user = await _userRepository.GetByUsername(username ?? string.Empty).FirstOrDefaultAsync(x => x.Username == username);
        if (user == null) return;
        if (user.ImagePath != null) await _fileService.DeleteFile(user.ImagePath);
        user.ImagePath = null;
        await _userRepository.SaveAsync();
    }
}