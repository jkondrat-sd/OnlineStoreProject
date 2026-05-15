using Confluent.Kafka;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaymentService.Application.Commands.ProcessOrderPayment;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Application.Events.IntegrationEvents;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure.Messaging;

public class OrderCreatedConsumerHostedService : BackgroundService
{
    private readonly KafkaSettings _settings;
    private readonly ILogger<OrderCreatedConsumerHostedService> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly IServiceProvider _serviceProvider;

    public OrderCreatedConsumerHostedService(
        IOptions<KafkaSettings> opts,
        ILogger<OrderCreatedConsumerHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _settings = opts.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.ConsumerGroupId + "-payment",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();
        _consumer.Subscribe(_settings.OrderCreatedTopic);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderCreatedConsumerHostedService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = _consumer.Consume(TimeSpan.FromSeconds(1));
                if (cr != null)
                {
                    _logger.LogInformation("Received message at {TopicPartitionOffset}", cr.TopicPartitionOffset);

                    var evt = JsonSerializer.Deserialize<OrderCreatedIntegrationEvent>(cr.Message.Value);
                    if (evt != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();

                        if (await db.ProcessedEvents.AnyAsync(e => e.OrderId == evt.OrderId, stoppingToken))
                        {
                            _logger.LogInformation("Event for OrderId {Evt} already processed", evt.OrderId);
                            _consumer.Commit(cr);
                            continue;
                        }

                        foreach (var account in db.Accounts)
                        {
                            _logger.LogInformation("Account {acc}, money {money}{currency}", account.UserId, account.Balance.Amount, account.Balance.Currency);
                        }
                        var cmd = new ProcessOrderPaymentCommand(
                            evt.UserId,
                            evt.OrderId,
                            evt.Amount,
                            evt.Currency);

                        await mediator.Send(cmd, stoppingToken);

                        db.ProcessedEvents.Add(new ProcessedEvent
                        {
                            OrderId = evt.OrderId,
                            ProcessedOn = DateTime.UtcNow
                        });
                        await db.SaveChangesAsync(stoppingToken);

                        _consumer.Commit(cr);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to deserialize OrderCreatedIntegrationEvent: {Message}", cr.Message.Value);
                    }
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
                _logger.LogError(ex, "Error consuming OrderCreated event");
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

        _logger.LogInformation("OrderCreatedConsumerHostedService stopping");
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}