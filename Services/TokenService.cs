using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Book.App.Models;
using Microsoft.IdentityModel.Tokens;

namespace Book.App.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Generate(string username, List<RoleModel> roles, int id, string? email, int agencyId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:JwtSecret"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim("AgencyId", agencyId.ToString()),
            new Claim(ClaimTypes.Email, email ?? string.Empty)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Role.ToString()));
        }

        var tokenDescriptor = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: _configuration["Jwt:Expires"] == null ? DateTime.Now.AddMinutes(15) : DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:Expires"])),
            signingCredentials: credentials
        );

        return tokenHandler.WriteToken(tokenDescriptor);
    }

    public ClaimsPrincipal GetPrincipal(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:JwtSecret"]);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        return tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
    }
}