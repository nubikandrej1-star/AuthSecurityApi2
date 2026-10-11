using System.Security.Claims;
using AuthSecurityApi.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthSecurityApi.Data;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentityDataSeeder");

        // 1. Автоматичне застосування міграцій
        await context.Database.MigrateAsync();

        // 2. Сідінг ролей
        string[] roleNames = { "Admin", "Manager", "User" };
        var roleDescriptions = new Dictionary<string, string>
        {
            ["Admin"] = "Повний доступ до адміністрування системи, користувачів та видалення замовлень",
            ["Manager"] = "Доступ до перегляду всіх замовлень клієнтів та звітів",
            ["User"] = "Стандартний клієнтський доступ до власного профілю та замовлень"
        };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new ApplicationRole(roleName, roleDescriptions[roleName]);
                var roleResult = await roleManager.CreateAsync(role);
                if (roleResult.Succeeded)
                {
                    logger.LogInformation("Створено роль: {RoleName}", roleName);
                }
            }
        }

        // 3. Сідінг користувачів
        var seedUsers = new[]
        {
            new
            {
                Email = "admin@authsecurity.com",
                FirstName = "Олександр",
                LastName = "Адміністратор",
                Password = "AdminPassword123!",
                IsVip = true,
                Roles = new[] { "Admin", "Manager", "User" },
                Claims = new[] { new Claim("MembershipType", "VIP"), new Claim("Department", "IT") }
            },
            new
            {
                Email = "manager@authsecurity.com",
                FirstName = "Ірина",
                LastName = "Менеджер",
                Password = "ManagerPassword123!",
                IsVip = false,
                Roles = new[] { "Manager", "User" },
                Claims = new[] { new Claim("Department", "Sales") }
            },
            new
            {
                Email = "vip@authsecurity.com",
                FirstName = "Віктор",
                LastName = "ВІП-Клієнт",
                Password = "UserPassword123!",
                IsVip = true,
                Roles = new[] { "User" },
                Claims = new[] { new Claim("MembershipType", "VIP") }
            },
            new
            {
                Email = "user@authsecurity.com",
                FirstName = "Максим",
                LastName = "Користувач",
                Password = "UserPassword123!",
                IsVip = false,
                Roles = new[] { "User" },
                Claims = Array.Empty<Claim>()
            }
        };

        foreach (var u in seedUsers)
        {
            var existingUser = await userManager.FindByEmailAsync(u.Email);
            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = u.Email,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    EmailConfirmed = true,
                    IsActive = true,
                    IsVip = u.IsVip,
                    CreatedAt = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(user, u.Password);
                if (createResult.Succeeded)
                {
                    logger.LogInformation("Створено користувача: {Email}", u.Email);

                    // Призначення ролей
                    await userManager.AddToRolesAsync(user, u.Roles);

                    // Додавання claims
                    if (u.Claims.Length > 0)
                    {
                        await userManager.AddClaimsAsync(user, u.Claims);
                    }
                }
                else
                {
                    logger.LogError("Помилка створення {Email}: {Errors}", u.Email, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
