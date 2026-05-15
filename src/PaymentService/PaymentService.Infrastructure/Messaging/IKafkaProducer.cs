namespace PaymentService.Infrastructure.Messaging;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string key, string payload);
}