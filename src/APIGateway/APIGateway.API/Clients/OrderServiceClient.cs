using APIGateway.API.DTOs;

namespace APIGateway.API.Clients;

public class OrderServiceClient : IOrderServiceClient
{
    private readonly HttpClient _http;
    private readonly ILogger<OrderServiceClient> _logger;

    public OrderServiceClient(HttpClient http, ILogger<OrderServiceClient> logger)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CreateOrderResponse?> CreateOrderAsync(CreateOrderRequest req)
    {
        try
        {
            var url = $"/api/orders?userId={req.UserId}";
            var body = new { req.Items };

            var resp = await _http.PostAsJsonAsync(url, body);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<CreateOrderResponse>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to create order for user {UserId} with request {@Request}", req.UserId, req.Items);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating order for user {UserId} with request {@Request}", req.UserId, req.Items);
            return null;
        }
    }

    public async Task<IReadOnlyList<OrderSummaryResponse>?> GetOrdersByUserAsync(Guid userId)
    {
        try
        {
            var resp = await _http.GetAsync($"/api/orders?userId={userId}");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<IReadOnlyList<OrderSummaryResponse>>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to retrieve orders for user {UserId}", userId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving orders for user {UserId}", userId);
            return null;
        }
    }

    public async Task<OrderDetailResponse?> GetOrderByIdAsync(Guid orderId)
    {
        try
        {
            var resp = await _http.GetAsync($"/api/orders/{orderId}");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<OrderDetailResponse>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to retrieve order details for order {OrderId}", orderId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving order details for order {OrderId}", orderId);
            return null;
        }
    }
}
