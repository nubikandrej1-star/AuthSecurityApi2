using System.ComponentModel.DataAnnotations;

namespace AuthSecurityApi.DTOs;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email є обов'язковим")]
    [EmailAddress(ErrorMessage = "Некоректний формат Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Пароль є обов'язковим")]
    [MinLength(6, ErrorMessage = "Пароль має містити щонайменше 6 символів")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ім'я є обов'язковим")]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Прізвище є обов'язковим")]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;
}
