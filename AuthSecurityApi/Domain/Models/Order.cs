namespace AuthSecurityApi.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // Зовнішній ключ на користувача, який оформив замовлення
        public int UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
}
