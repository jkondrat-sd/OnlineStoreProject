namespace APIGateway.API.DTOs;

public class CreateOrderApiRequest
{
    public Guid UserId { get; set; }
    public List<CreateOrderItemApiDto> Items { get; set; }
}