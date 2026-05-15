namespace OrderService.Infrastructure.Messaging;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string OrderCreatedTopic { get; set; }
    public string OrderStatusChangedTopic { get; set; }
    public string PaymentSucceededTopic { get; set; }
    public string PaymentFailedTopic { get; set; }
    public string ConsumerGroupId { get; set; }
}