using Microsoft.AspNetCore.Identity;

namespace AuthSecurityApi.Domain.Models;

public class ApplicationUser : IdentityUser<int>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsVip { get; set; } = false;

    // Додайте до класу ApplicationUser:
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
