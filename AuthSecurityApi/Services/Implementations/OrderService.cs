using AuthSecurityApi.Data;
using AuthSecurityApi.Domain.Models;
using AuthSecurityApi.DTOs;
using AuthSecurityApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthSecurityApi.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly AppIdentityDbContext _context;

    public OrderService(AppIdentityDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(order => order.User)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(int userId)
    {
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(order => order.User)
            .Where(order => order.UserId == userId)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();

        return orders.Select(MapToResponse).ToList();
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(int orderId)
    {
        var order = await _context.Orders
            .AsNoTracking()
            .Include(item => item.User)
            .FirstOrDefaultAsync(item => item.Id == orderId);

        return order is null ? null : MapToResponse(order);
    }

    public async Task<OrderResponse> CreateOrderAsync(
        int userId,
        CreateOrderRequest request,
        decimal discountPercent = 0)
    {
        if (discountPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(discountPercent), "Знижка має бути в межах від 0 до 100 відсотків.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);
        if (user is null)
        {
            throw new KeyNotFoundException("Користувача, який створює замовлення, не знайдено.");
        }

        var finalAmount = discountPercent > 0
            ? Math.Round(request.TotalAmount * (1 - (discountPercent / 100m)), 2)
            : request.TotalAmount;

        var order = new Order
        {
            Description = request.Description.Trim(),
            TotalAmount = finalAmount,
            OrderDate = DateTime.UtcNow,
            UserId = userId,
            User = user
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return MapToResponse(order);
    }

    public async Task<bool> DeleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order is null)
        {
            return false;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            Description = order.Description,
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            UserId = order.UserId,
            CustomerEmail = order.User?.Email ?? string.Empty,
            CustomerName = $"{order.User?.FirstName} {order.User?.LastName}".Trim()
        };
    }
}
