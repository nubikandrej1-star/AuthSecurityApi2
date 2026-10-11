using AuthSecurityApi.DTOs;

namespace AuthSecurityApi.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderResponse>> GetAllOrdersAsync();
    Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(int userId);
    Task<OrderResponse?> GetOrderByIdAsync(int orderId);
    Task<OrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request, decimal discountPercent = 0);
    Task<bool> DeleteOrderAsync(int orderId);
}
