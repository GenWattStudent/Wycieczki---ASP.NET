using System.Security.Claims;
using Book.App.Models;

namespace Book.App.Services;

public interface ITokenService
{
    string GenerateToken(string username, Role role, int id, string? imagePath, string? email);
    ClaimsPrincipal GetPrincipal(string token);
}