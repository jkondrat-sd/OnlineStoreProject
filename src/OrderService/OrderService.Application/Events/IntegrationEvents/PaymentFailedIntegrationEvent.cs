using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Events.IntegrationEvents;

public record PaymentFailedIntegrationEvent( Guid OrderId, Guid AccountId, decimal Amount, string Currency, string Reason );