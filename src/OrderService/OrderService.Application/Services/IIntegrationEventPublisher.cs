namespace OrderService.Application.Services;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(object integrationEvent, CancellationToken ct = default);
}