using OrderService.Domain.Entities;

namespace OrderService.Application.Queries.GetOrderById;

public class OrderDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public decimal TotalAmount { get; init; }
    public string Currency { get; init; }
    public string Status { get; init; }
    public List<OrderItemDto> Items { get; set; } = [];
    public DateTime CreatedAt { get; init; }
}