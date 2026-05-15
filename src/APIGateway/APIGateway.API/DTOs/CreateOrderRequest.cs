namespace APIGateway.API.DTOs;

public class CreateOrderRequest
{
    public Guid UserId { get; set; }
    public List<OrderItemDto> Items { get; set; }
}