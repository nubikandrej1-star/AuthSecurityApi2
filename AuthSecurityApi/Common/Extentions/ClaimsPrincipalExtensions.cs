using System.Security.Claims;

namespace AuthSecurityApi.Common.Extentions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(id, out int userId) ? userId : null;
        }

        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static IEnumerable<string> GetUserRoles(this ClaimsPrincipal user)
        {
            return user.FindAll(ClaimTypes.Role).Select(r=>r.Value);
        }

        public static bool IsVip(this ClaimsPrincipal user)
        {
            return user.HasClaim(x => x.Type == "MembershipType" && x.Value == "VIP");
        }
    }
}
