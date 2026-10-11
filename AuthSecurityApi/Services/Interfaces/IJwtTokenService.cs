using AuthSecurityApi.Domain.Models;
using System.Security.Claims;

namespace AuthSecurityApi.Services.Interfaces;

public interface IJwtTokenService
{
    public string GenerateToken(ApplicationUser user, IList<string> roles, IList<Claim> userClaims);
    public DateTime GetExpirationDate();
}