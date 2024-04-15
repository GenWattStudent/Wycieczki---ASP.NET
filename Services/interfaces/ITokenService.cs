using System.Security.Claims;
using Book.App.Models;

namespace Book.App.Services;

public interface ITokenService
{
    string Generate(string username, List<RoleModel> role, int id, string? imagePath, string? email);
    ClaimsPrincipal GetPrincipal(string token);
}