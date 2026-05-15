using System.Text.Json.Serialization;

namespace APIGateway.API.DTOs;

public class OrderSummaryResponse
{
    [JsonPropertyName("Id")]
    public Guid OrderId     { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status      { get; set; }
    public DateTime CreatedAt { get; set; }
}