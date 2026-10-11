using AuthSecurityApi.Domain.Models;
using AuthSecurityApi.DTOs;
using AuthSecurityApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace AuthSecurityApi.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<AuthService> _logger;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Користувач із таким Email вже існує",
                Errors = new[] { "Email already taken" }
            };
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            IsVip = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Помилка реєстрації",
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        await _userManager.AddToRoleAsync(user, "User");
        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation("Успішно зареєстровано нового користувача {Email}", user.Email);

        return new AuthResponse
        {
            Success = true,
            Message = "Реєстрація успішна",
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Roles = roles
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Невірний email або пароль",
                Errors = new[] { "Invalid credentials or account inactive" }
            };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Обліковий запис заблоковано через забагато невдалих спроб",
                    Errors = new[] { "Account locked out" }
                };
            }

            return new AuthResponse
            {
                Success = false,
                Message = "Невірний email або пароль",
                Errors = new[] { "Invalid credentials" }
            };
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userClaims = await _userManager.GetClaimsAsync(user);
        var expiresAt = _jwtTokenService.GetExpirationDate();
        var token = _jwtTokenService.GenerateToken(user, roles, userClaims);

        _logger.LogInformation("Успішний вхід користувача {Email}", user.Email);

        return new AuthResponse
        {
            Success = true,
            Message = "Вхід успішний",
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = $"{user.FirstName} {user.LastName}".Trim(),
            Roles = roles
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return result.Succeeded;
    }
}
