using System.Text.Json;
using PaymentService.Application.Services;
using PaymentService.Infrastructure.Entities;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure.EventPublishing;

public class OutboxIntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly PaymentDbContext _ctx;
    public OutboxIntegrationEventPublisher(PaymentDbContext ctx) => _ctx = ctx;

    public async Task PublishAsync(object integrationEvent, CancellationToken ct = default)
    {
        var msg = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            OccurredOn = DateTime.UtcNow,
            EventType = integrationEvent.GetType().Name,
            Payload = JsonSerializer.Serialize(integrationEvent),
            SentOn = null
        };
        await _ctx.OutboxMessages.AddAsync(msg, ct);
    }
}