namespace APIGateway.API.DTOs;

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity   { get; set; }
    public decimal Price  { get; set; }
    public string Currency{ get; set; }
}