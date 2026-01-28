using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FrenzyNet.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace FrenzyNet.Api.Services;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(User user)
    {
        var secret = _configuration["JWT_SECRET"] ?? "dev-secret-change";
        var issuer = _configuration["JWT_ISSUER"] ?? "frenzynet";
        var audience = _configuration["JWT_AUDIENCE"] ?? "frenzynet-web";

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("username", user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
