using Confluent.Kafka; using MediatR; using Microsoft.Extensions.Hosting; using Microsoft.Extensions.Logging; using Microsoft.Extensions.Options; using OrderService.Application.Commands.ProcessPaymentResult; using OrderService.Application.Events.IntegrationEvents; using System.Text.Json;

namespace OrderService.Infrastructure.Messaging;

public class PaymentResultConsumerHostedService : BackgroundService {
    private readonly KafkaSettings _settings;
    private readonly IMediator _mediator;
    private readonly ILogger _logger;
    private readonly IConsumer<string, string> _consumer;

public PaymentResultConsumerHostedService(
    IOptions<KafkaSettings> opts,
    IMediator mediator,
    ILogger<PaymentResultConsumerHostedService> logger)
{
    _settings = opts.Value;
    _mediator = mediator;
    _logger = logger;

    var config = new ConsumerConfig
    {
        BootstrapServers = _settings.BootstrapServers,
        GroupId = _settings.ConsumerGroupId + "-order",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnableAutoCommit = false
    };

    _consumer = new ConsumerBuilder<string, string>(config).Build();
    _consumer.Subscribe(new[] { _settings.PaymentSucceededTopic, _settings.PaymentFailedTopic });
}

protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    _logger.LogInformation("PaymentResultConsumerHostedService started");

    while (!stoppingToken.IsCancellationRequested)
    {
        try
        {
            var cr = _consumer.Consume(TimeSpan.FromSeconds(1));
            if (cr != null)
            {
                _logger.LogDebug("Received message at {TopicPartitionOffset}", cr.TopicPartitionOffset);

                if (cr.Topic == _settings.PaymentSucceededTopic)
                {
                    var evt = JsonSerializer.Deserialize<PaymentSucceededIntegrationEvent>(cr.Message.Value);
                    if (evt != null)
                    {
                        await _mediator.Send(new ProcessPaymentResultCommand(evt.OrderId, true, null), stoppingToken);
                        _logger.LogInformation("Processed payment succeeded for OrderId={OrderId}", evt.OrderId);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to deserialize PaymentSucceededIntegrationEvent: {Message}", cr.Message.Value);
                    }
                }
                else if (cr.Topic == _settings.PaymentFailedTopic)
                {
                    var evt = JsonSerializer.Deserialize<PaymentFailedIntegrationEvent>(cr.Message.Value);
                    if (evt != null)
                    {
                        await _mediator.Send(new ProcessPaymentResultCommand(evt.OrderId, false, evt.Reason), stoppingToken);
                        _logger.LogInformation("Processed payment failed for OrderId={OrderId} with reason: {Reason}", evt.OrderId, evt.Reason);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to deserialize PaymentFailedIntegrationEvent: {Message}", cr.Message.Value);
                    }
                }
                else
                {
                    _logger.LogWarning("Received message from unknown topic: {Topic}", cr.Topic);
                }
                _consumer.Commit(cr);
            }
            else
            {
                await Task.Delay(500, stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            break;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming payment result");
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    _logger.LogInformation("PaymentResultConsumerHostedService stopping");
}

public override void Dispose()
{
    _consumer.Close();
    _consumer.Dispose();
    base.Dispose();
}

}