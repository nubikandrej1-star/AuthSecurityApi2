using AuthSecurityApi.Domain.Models;
using AuthSecurityApi.DTOs;
using AuthSecurityApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthSecurityApi.Services.Implementations;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                IsVip = user.IsVip,
                CreatedAt = user.CreatedAt,
                Roles = roles
            });
        }

        return result;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsActive = user.IsActive,
            IsVip = user.IsVip,
            CreatedAt = user.CreatedAt,
            Roles = roles
        };
    }

    public async Task<UserProfileResponse?> GetUserProfileAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        var claimsDict = claims.ToDictionary(c => c.Type, c => c.Value);

        return new UserProfileResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            IsVip = user.IsVip,
            Roles = roles,
            Claims = claimsDict
        };
    }

    public async Task<bool> AssignRoleAsync(int userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        if (!await _roleManager.RoleExistsAsync(roleName)) return false;

        if (!await _userManager.IsInRoleAsync(user, roleName))
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result.Succeeded;
        }

        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return false;

        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }
}
