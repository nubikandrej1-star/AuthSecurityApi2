using System.ComponentModel.DataAnnotations;

namespace AuthSecurityApi.DTOs;

public class AssignRoleRequest
{
    [Required(ErrorMessage = "Назва ролі є обов'язковою")]
    public string RoleName { get; set; } = string.Empty;
}
