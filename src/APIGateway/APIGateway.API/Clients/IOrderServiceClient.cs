using APIGateway.API.DTOs;

namespace APIGateway.API.Clients;

public interface IOrderServiceClient
{
    Task<CreateOrderResponse?> CreateOrderAsync(CreateOrderRequest req);
    Task<IReadOnlyList<OrderSummaryResponse>?> GetOrdersByUserAsync(Guid userId);
    Task<OrderDetailResponse?> GetOrderByIdAsync(Guid orderId);
}