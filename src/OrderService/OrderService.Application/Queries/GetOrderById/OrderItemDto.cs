namespace OrderService.Application.Queries.GetOrderById;

public class OrderItemDto
{
    public Guid   ProductId { get; init; }
    public int    Quantity  { get; init; }
    public decimal Price     { get; init; }
    public string Currency  { get; init; }
}