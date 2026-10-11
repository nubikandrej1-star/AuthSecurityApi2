using AuthSecurityApi.Domain.Models;
using AuthSecurityApi.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthSecurityApi.Services.Implementations;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(ApplicationUser user, IList<string> roles, IList<Claim> userClaims)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey");
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expirationDate = GetExpirationDate();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim("IsVip", user.IsVip.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var claim in userClaims)
        {
            if (!claims.Any(c => c.Type == claim.Type && c.Value == claim.Value))
            {
                claims.Add(claim);
            }
        }

        var token = new JwtSecurityToken(issuer, audience, claims, DateTime.UtcNow, expirationDate, credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTime GetExpirationDate()
    {
        var expiryMinutes = int.TryParse(_configuration["JwtSettings:ExpiryMinutes"], out var expiryMin) ? expiryMin : 60;
        return DateTime.UtcNow.AddMinutes(expiryMinutes);
    }
}