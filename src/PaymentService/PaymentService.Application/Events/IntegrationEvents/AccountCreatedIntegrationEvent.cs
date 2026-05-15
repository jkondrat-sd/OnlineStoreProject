namespace PaymentService.Application.Events.IntegrationEvents;
public record AccountCreatedIntegrationEvent(Guid AccountId, Guid UserId);