namespace APIGateway.API.DTOs;

public class CreateOrderItemApiDto
{
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; }
}