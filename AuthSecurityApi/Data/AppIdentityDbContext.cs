using AuthSecurityApi.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthSecurityApi.Data;

public class AppIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public DbSet<Order> Orders => Set<Order>();

    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(u => u.IsVip).HasDefaultValue(false);
        });

        builder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Description).HasMaxLength(500).IsRequired();
            entity.Property(o => o.TotalAmount).HasPrecision(18, 2).IsRequired();
            entity.Property(o => o.OrderDate).HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(o => o.User)
                  .WithMany(u => u.Orders)
                  .HasForeignKey(o => o.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
