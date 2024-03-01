using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Book.App.Models;
using Microsoft.IdentityModel.Tokens;

namespace Book.App.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string username, Role role, int id)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role.ToString()),
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        };

        var tokenDescriptor = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return tokenHandler.WriteToken(tokenDescriptor);
    }

    public ClaimsPrincipal GetPrincipal(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
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