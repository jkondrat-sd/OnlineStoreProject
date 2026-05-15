namespace OrderService.API.DTOs;

public class CreateOrderResponse
{
    public Guid OrderId { get; set; }
    public string Status  { get; set; }
}