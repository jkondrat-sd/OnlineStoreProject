namespace OrderService.Application.Events.IntegrationEvents;

public class OrderCreatedIntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    public OrderCreatedIntegrationEvent(Guid orderId, Guid userId, decimal amount, string currency)
    {
        OrderId = orderId; UserId = userId;
        Amount = amount; Currency = currency;
    }
}