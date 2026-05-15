using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Events.IntegrationEvents;
public record FundsDepositedIntegrationEvent(Guid AccountId, Money money);