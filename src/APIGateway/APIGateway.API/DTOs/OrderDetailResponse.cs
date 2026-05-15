using System.Text.Json.Serialization;

namespace APIGateway.API.DTOs;

public class OrderDetailResponse
{
    
    [JsonPropertyName("Id")]
    public Guid OrderId     { get; set; }
    public Guid UserId      { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency    { get; set; }
    public string Status      { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<OrderItemDto> Items { get; set; }
}