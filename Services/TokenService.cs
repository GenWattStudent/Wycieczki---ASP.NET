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

    public string Generate(string username, List<RoleModel> roles, int id, string? imagePath, string? email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim("ImagePath", imagePath ?? string.Empty),
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
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
        );

        return tokenHandler.WriteToken(tokenDescriptor);
    }

    public ClaimsPrincipal GetPrincipal(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET"));
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