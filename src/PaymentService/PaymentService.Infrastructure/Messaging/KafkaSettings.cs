namespace PaymentService.Infrastructure.Messaging;


public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string OrderCreatedTopic { get; set; }
    public string ConsumerGroupId { get; set; }
}