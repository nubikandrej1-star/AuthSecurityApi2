using AuthSecurityApi.DTOs;

namespace AuthSecurityApi.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
}
