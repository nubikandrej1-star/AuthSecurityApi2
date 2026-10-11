using AuthSecurityApi.Common.Extentions;
using AuthSecurityApi.DTOs;
using AuthSecurityApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthSecurityApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Ok(orders);
    }

    [HttpGet("my-orders")]
    [Authorize]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var orders = await _orderService.GetOrdersByUserIdAsync(userId.Value);
        return Ok(orders);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order is null)
        {
            return NotFound();
        }

        var userId = User.GetUserId();
        var canViewAllOrders = User.IsInRole("Admin") || User.IsInRole("Manager");
        if (userId is null)
        {
            return Unauthorized();
        }

        if (!canViewAllOrders && order.UserId != userId.Value)
        {
            return NotFound();
        }

        return Ok(order);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var order = await _orderService.CreateOrderAsync(userId.Value, request);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return Unauthorized();
        }
    }

    [HttpPost("vip-discount")]
    [Authorize(Policy = "VipOnly")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateVipOrder([FromBody] CreateOrderRequest request)
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        try
        {
            var order = await _orderService.CreateOrderAsync(userId.Value, request, discountPercent: 15m);
            return Ok(order);
        }
        catch (KeyNotFoundException)
        {
            return Unauthorized();
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteOrder(int id)
    {
        var deleted = await _orderService.DeleteOrderAsync(id);
        if (!deleted)
        {
            return NotFound(new { message = "Замовлення не знайдено." });
        }

        return Ok(new { message = "Замовлення успішно видалено." });
    }
}
