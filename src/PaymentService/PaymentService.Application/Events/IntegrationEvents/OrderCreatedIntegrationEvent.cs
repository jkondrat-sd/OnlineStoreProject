namespace PaymentService.Application.Events.IntegrationEvents;

public record OrderCreatedIntegrationEvent(
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    string Currency
);