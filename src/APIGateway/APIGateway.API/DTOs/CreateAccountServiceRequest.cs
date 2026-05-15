namespace APIGateway.API.DTOs;

public class CreateAccountServiceRequest
{
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public string Currency { get; set; } = null!;
}