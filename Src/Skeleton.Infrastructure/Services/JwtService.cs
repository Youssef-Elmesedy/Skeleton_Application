using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.Entities;

namespace Skeleton.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    public JwtService(IConfiguration config) => _config = config;

    public string GenerateToken(AppUser user)
    {
        var jwt     = _config.GetSection("JwtSettings");
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SecretKey"]!));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwt["ExpiresInMinutes"]!));

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name,           user.Username),
            new(ClaimTypes.Email,          user.Email),
            new(ClaimTypes.Role,           user.Role.ToString()),
            new("EmailVerified",           user.IsEmailVerified.ToString())
        };

        if (user.CustomerId.HasValue)
            claims.Add(new Claim("CustomerId", user.CustomerId.Value.ToString()));

        if (user.EmployeeId.HasValue)
            claims.Add(new Claim("EmployeeId", user.EmployeeId.Value.ToString()));

        var token = new JwtSecurityToken(
            issuer:             jwt["Issuer"],
            audience:           jwt["Audience"],
            claims:             claims,
            expires:            expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
