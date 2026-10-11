using AuthSecurityApi.DTOs;

namespace AuthSecurityApi.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserProfileResponse?> GetUserProfileAsync(int id);
    Task<bool> AssignRoleAsync(int userId, string roleName);
    Task<bool> DeleteUserAsync(int id);
}
