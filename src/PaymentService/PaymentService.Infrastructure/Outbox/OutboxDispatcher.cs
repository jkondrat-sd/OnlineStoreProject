using Microsoft.EntityFrameworkCore;
using PaymentService.Infrastructure.Messaging;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure.Outbox;

public class OutboxDispatcher : IOutboxDispatcher
{
    private readonly PaymentDbContext _ctx;
    private readonly IKafkaProducer _producer;
    private readonly KafkaSettings _settings;

    public OutboxDispatcher(
        PaymentDbContext ctx,
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
            string topic = msg.EventType switch
            {
                "AccountCreatedIntegrationEvent"       => "account-created",
                "PaymentSucceededIntegrationEvent" => "payment-succeeded",
                "PaymentFailedIntegrationEvent"    => "payment-failed",
                "FundsDepositedIntegrationEvent"    => "funds-deposited",
                _ => throw new InvalidOperationException($"Unknown event {msg.EventType}")
            };
            await _producer.ProduceAsync(topic, msg.Id.ToString(), msg.Payload);
            msg.SentOn = DateTime.UtcNow;
        }
        await _ctx.SaveChangesAsync(ct);
    }
}