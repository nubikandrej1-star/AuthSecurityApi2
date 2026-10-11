using AuthSecurityApi.DTOs;
using AuthSecurityApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSecurityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Тільки користувачі з роллю "Admin" мають доступ до цього контролера
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Отримання списку всіх користувачів системи (Тільки Admin)
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Отримання користувача за ID (Тільки Admin)
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { message = $"Користувача з ID {id} не знайдено" });
        }

        return Ok(user);
    }

    /// <summary>
    /// Призначення нової ролі користувачу (Тільки Admin)
    /// </summary>
    [HttpPost("{id:int}/assign-role")]
    public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _userService.AssignRoleAsync(id, request.RoleName);
        if (!result)
        {
            return BadRequest(new { message = $"Не вдалося призначити роль '{request.RoleName}'. Перевірте існування користувача та ролі." });
        }

        return Ok(new { message = $"Роль '{request.RoleName}' успішно призначена користувачу ID {id}" });
    }

    /// <summary>
    /// Видалення облікового запису користувача (Тільки Admin)
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"Користувача з ID {id} не знайдено або не вдалося видалити" });
        }

        return Ok(new { message = $"Користувача ID {id} успішно видалено" });
    }
}
