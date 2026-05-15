using Confluent.Kafka;
using Confluent.Kafka.Admin;

namespace PaymentService.Infrastructure.Messaging;

public class KafkaTopicCreator
{
    private readonly KafkaSettings _settings;

    public KafkaTopicCreator(KafkaSettings settings)
    {
        _settings = settings;
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
            new() { Name = "order-created", NumPartitions = 1, ReplicationFactor = 1 },
            new() { Name = "order-status-changed", NumPartitions = 1, ReplicationFactor = 1 },
            new() { Name = "account-created", NumPartitions = 1, ReplicationFactor = 1 },
            new() { Name = "funds-deposited", NumPartitions = 1, ReplicationFactor = 1 },
            new() { Name = "payment-succeeded", NumPartitions = 1, ReplicationFactor = 1 },
            new() { Name = "payment-failed", NumPartitions = 1, ReplicationFactor = 1 }
        };


        try
        {
            await adminClient.CreateTopicsAsync(topics);
            Console.WriteLine("Topics created successfully.");
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