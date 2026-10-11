using System.ComponentModel.DataAnnotations;

namespace AuthSecurityApi.DTOs;

public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Поточний пароль є обов'язковим")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Новий пароль є обов'язковим")]
    [MinLength(6, ErrorMessage = "Новий пароль повинен містити щонайменше 6 символів")]
    public string NewPassword { get; set; } = string.Empty;
}
