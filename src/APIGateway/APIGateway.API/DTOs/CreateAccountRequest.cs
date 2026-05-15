namespace APIGateway.API.DTOs;

public class CreateAccountRequest
{
    public decimal Balance { get; set; }
    public string Currency { get; set; }
}