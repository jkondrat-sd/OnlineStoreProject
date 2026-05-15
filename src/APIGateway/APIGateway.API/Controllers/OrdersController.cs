using APIGateway.API.Clients;
using APIGateway.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderServiceClient _orders;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderServiceClient orders, ILogger<OrdersController> logger)
    {
        _orders = orders;
        _logger = logger;
    }

    [HttpPost("create-order")]
    public async Task<ActionResult<CreateOrderResponse>> Create([FromBody] CreateOrderApiRequest req)
    {
        try
        {
            var serviceReq = new CreateOrderRequest
            {
                UserId = req.UserId,
                Items = req.Items.Select(item => new OrderItemDto
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Currency = item.Currency
                }).ToList()
            };
            var res = await _orders.CreateOrderAsync(serviceReq);
            if (res == null)
            {
                _logger.LogWarning("Failed to create order for request {@Request}", req);
                return BadRequest("Failed to create order");
            }

            return CreatedAtAction(nameof(GetById), new { orderId = res.OrderId }, res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating order for request {@Request}", req);
            return StatusCode(500, "An error occurred while creating the order");
        }
    }

    [HttpGet("get-orders/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<OrderSummaryResponse>>> GetByUser(Guid userId)
    {
        try
        {
            var res = await _orders.GetOrdersByUserAsync(userId);
            if (res == null)
            {
                _logger.LogWarning("Failed to retrieve orders for user {UserId}", userId);
                return NotFound("No orders found for the user");
            }

            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving orders for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the orders");
        }
    }

    [HttpGet("get-order/{orderId:guid}")]
    public async Task<ActionResult<OrderDetailResponse>> GetById(Guid orderId)
    {
        try
        {
            var res = await _orders.GetOrderByIdAsync(orderId);
            if (res == null)
            {
                _logger.LogWarning("Failed to retrieve order details for order {OrderId}", orderId);
                return NotFound("Order not found");
            }

            return Ok(res);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving order details for order {OrderId}", orderId);
            return StatusCode(500, "An error occurred while retrieving the order details");
        }
    }
}