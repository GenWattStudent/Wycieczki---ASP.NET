using System.Security.Claims;
using Book.App.Models;

namespace Book.App.Services;

public interface ITokenService
{
    string Generate(string username, List<RoleModel> role, int id, string? email, int agencyId);
    ClaimsPrincipal GetPrincipal(string token);
}