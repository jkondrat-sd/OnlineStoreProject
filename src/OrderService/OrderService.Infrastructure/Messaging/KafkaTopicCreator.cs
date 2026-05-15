using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OrderService.Infrastructure.Messaging;

public class KafkaTopicCreator
{
    private readonly KafkaSettings _settings;
    private readonly ILogger<KafkaTopicCreator> _logger;

    public KafkaTopicCreator(IOptions<KafkaSettings> opts, ILogger<KafkaTopicCreator> logger)
    {
        _settings = opts.Value;
        _logger = logger;
    }

    public async Task CreateTopicsAsync()
    {
        var config = new AdminClientConfig
        {
            BootstrapServers = _settings.BootstrapServers
        };

        using var adminClient = new AdminClientBuilder(config).Build();

        var topics = new List<TopicSpecification>
        {
            new() { Name = _settings.OrderCreatedTopic, NumPartitions = 1, ReplicationFactor = 1 },
            new() { Name = _settings.OrderStatusChangedTopic, NumPartitions = 1, ReplicationFactor = 1 },
            new() { Name = _settings.PaymentSucceededTopic, NumPartitions = 1, ReplicationFactor = 1 },
            new() { Name = _settings.PaymentFailedTopic, NumPartitions = 1, ReplicationFactor = 1 }
        };

        try
        {
            await adminClient.CreateTopicsAsync(topics);
            Console.WriteLine("Topics created successfully");
        }
        catch (CreateTopicsException e)
        {
            foreach (var result in e.Results)
            {
                if (result.Error.Code == ErrorCode.NoError)
                {
                    Console.WriteLine($"Topic {result.Topic} created successfully");
                }
                else if (result.Error.Code == ErrorCode.TopicAlreadyExists)
                {
                    Console.WriteLine($"Topic {result.Topic} already exists");
                }
                else
                {
                    Console.WriteLine($"Error creating topic {result.Topic}: {result.Error.Reason}");
                }
            }
        }
    }
}