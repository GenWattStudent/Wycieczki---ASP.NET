using Book.App.Models;
using Book.App.ViewModels;

namespace Book.App.Services;

public interface IUserService
{
    Task Register(RegisterViewModel registerModel);
    Task EditUserInfo(EditUserViewModel userViewModel, UserModel userModel);
    Task RegisterAdmin(UserModel userModel);
    Task<UserModel?> GetByUsername(string username);
    Task Delete(string username);
    Task DeleteImage(string? username);
}