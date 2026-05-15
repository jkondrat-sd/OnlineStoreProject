using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.API.DTOs;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Queries.GetOrderById;
using OrderService.Application.Queries.GetOrdersByUser;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
    }

    // POST api/orders?userId={userId}
    [HttpPost]
    public async Task<ActionResult<OrderSummaryDto>> Create(
        [FromQuery] Guid userId,
        [FromBody] CreateOrderRequest items)
    {
        var cmd = new CreateOrderCommand(userId, items.Items);
        var result = await _mediator.Send(cmd);
        return CreatedAtAction(
            nameof(GetById),
            new { orderId = result.OrderId },
            result
        );
    }

    // GET api/orders?userId={userId}
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderSummaryDto>>> GetByUser(
        [FromQuery] Guid userId)
    {
        var list = await _mediator.Send(new GetOrdersByUserQuery(userId));
        return Ok(list);
    }

    // GET api/orders/{orderId}
    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid orderId)
    {
        var dto = await _mediator.Send(new GetOrderByIdQuery(orderId));
        return Ok(dto);
    }
    
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok();
    }
}