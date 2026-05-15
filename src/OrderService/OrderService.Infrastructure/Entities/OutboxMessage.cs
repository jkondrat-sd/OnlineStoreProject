namespace OrderService.Infrastructure.Entities;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; }
    public string EventType { get; set; }
    public string Payload { get; set; }
    public DateTime? SentOn { get; set; }
}