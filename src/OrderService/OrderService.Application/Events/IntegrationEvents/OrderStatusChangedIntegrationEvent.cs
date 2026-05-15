namespace OrderService.Application.Events.IntegrationEvents;

public class OrderStatusChangedIntegrationEvent
{
    public Guid OrderId { get; init; }
    public string NewStatus { get; init; }

    public OrderStatusChangedIntegrationEvent(Guid orderId, string newStatus)
    {
        OrderId = orderId; NewStatus = newStatus;
    }
}