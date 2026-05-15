using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Outbox;

public class OutboxDispatcher : IOutboxDispatcher
{
    private readonly OrderDbContext _ctx;
    private readonly IKafkaProducer _producer;
    private readonly KafkaSettings _settings;

    public OutboxDispatcher(
        OrderDbContext ctx,
        IKafkaProducer producer,
        KafkaSettings settings)
    {
        _ctx = ctx;
        _producer = producer;
        _settings = settings;
    }

    public async Task DispatchPendingAsync(CancellationToken ct = default)
    {
        var pendings = await _ctx.OutboxMessages
            .Where(m => m.SentOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(50)
            .ToListAsync(ct);
        foreach (var msg in pendings)
        {
            var topic = msg.EventType switch
            {
                "OrderCreatedIntegrationEvent"      => _settings.OrderCreatedTopic,
                "OrderStatusChangedIntegrationEvent" => _settings.OrderStatusChangedTopic,
                _ => throw new InvalidOperationException($"Unknown event {msg.EventType}")
            };

            await _producer.ProduceAsync(topic, msg.Id.ToString(), msg.Payload);
            msg.SentOn = DateTime.UtcNow;
        }

        await _ctx.SaveChangesAsync(ct);
    }
}