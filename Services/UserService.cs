using Book.App.Models;
using Book.App.Repositories;
using Book.App.ViewModels;

namespace Book.App.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly string _userImageFolder = "users";
    private readonly FileService _fileService;

    public UserService(IUserRepository userRepository, FileService fileService)
    {
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
        return await _userRepository.GetByUsername(username);
    }

    public async Task<UserModel?> Login(LoginModel loginModel)
    {
        var user = await _userRepository.GetByUsername(loginModel.Username);

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
        var user = await _userRepository.GetByUsername(username);
        if (user == null)
        {
            return;
        }

        await _userRepository.Delete(user.Id);
        if (user.ImagePath != null) await _fileService.DeleteFile(user.ImagePath);
        await _userRepository.SaveAsync();
    }

    public async Task DeleteImage(string? username)
    {
        var user = await _userRepository.GetByUsername(username ?? string.Empty);
        if (user == null) return;
        if (user.ImagePath != null) await _fileService.DeleteFile(user.ImagePath);
        user.ImagePath = null;
        await _userRepository.SaveAsync();
    }
}