namespace PaymentService.Domain.Entities;

public class ProcessedEvent
{
    public Guid OrderId { get; set; }
    public DateTime ProcessedOn { get; set; }
}