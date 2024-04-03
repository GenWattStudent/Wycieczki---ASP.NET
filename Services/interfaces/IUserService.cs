using Book.App.Models;

namespace Book.App.Services;

public interface IUserService
{
    Task Register(RegisterModel registerModel);
    Task EditUserInfo(RegisterModel userViewModel, UserModel userModel);

    Task RegisterAdmin(UserModel userModel);


    Task<UserModel?> GetByUsername(string username);

    Task<UserModel?> Login(LoginModel loginModel);

    Task Delete(string username);

    Task DeleteImage(string? username);
}