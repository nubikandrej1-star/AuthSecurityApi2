namespace AuthSecurityApi.DTOs;

public class UserProfileResponse
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsVip { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
    public IDictionary<string, string> Claims { get; set; } = new Dictionary<string, string>();
}
