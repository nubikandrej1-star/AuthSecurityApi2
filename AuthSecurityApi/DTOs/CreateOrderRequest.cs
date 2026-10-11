using System.ComponentModel.DataAnnotations;

namespace AuthSecurityApi.DTOs;

public class CreateOrderRequest
{
    [Required(ErrorMessage = "Опис замовлення є обов'язковим")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "Опис має містити від 3 до 500 символів")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Сума замовлення є обов'язковою")]
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Сума має бути від 0.01 до 1 000 000 грн")]
    public decimal TotalAmount { get; set; }
}
