namespace OrderService.Application.Queries.GetOrdersByUser;

public class OrderSummaryDto
{
    public Guid Id { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; }
    public DateTime CreatedAt { get; init; }
}