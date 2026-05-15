using Confluent.Kafka;

namespace OrderService.Infrastructure.Messaging;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaSettings _settings;

    public KafkaProducer(KafkaSettings settings)
    {
        _settings = settings;
        var config = new ProducerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string key, string payload)
    {
        var msg = new Message<string, string> { Key = key, Value = payload };
        await _producer.ProduceAsync(topic, msg);
    }
}