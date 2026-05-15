namespace PaymentService.Application.Events.IntegrationEvents;

public record PaymentSucceededIntegrationEvent(
    Guid OrderId,
    Guid AccountId,
    decimal Amount,
    string Currency);