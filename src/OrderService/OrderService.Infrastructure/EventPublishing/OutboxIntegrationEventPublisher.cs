using System.Text.Json;
using OrderService.Application.Services;
using OrderService.Infrastructure.Entities;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.EventPublishing;

public class OutboxIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly OrderDbContext _ctx;
    public OutboxIntegrationEventPublisher(OrderDbContext ctx) => _ctx = ctx;

    public async Task PublishAsync(object integrationEvent, CancellationToken ct = default)
    {
        var message = new OutboxMessage
        {
            Id         = Guid.NewGuid(),
            OccurredOn = DateTime.UtcNow,
            EventType  = integrationEvent.GetType().Name,
            Payload    = JsonSerializer.Serialize(integrationEvent),
            SentOn     = null
        };
        await _ctx.OutboxMessages.AddAsync(message, ct);
    }
}