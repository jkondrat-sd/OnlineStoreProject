namespace PaymentService.Application.Events.IntegrationEvents;

public record PaymentFailedIntegrationEvent(
    Guid OrderId,
    Guid AccountId,
    decimal Amount,
    string Currency,
    string Reason);