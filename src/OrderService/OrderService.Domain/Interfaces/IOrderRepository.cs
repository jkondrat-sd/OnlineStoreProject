using OrderService.Domain.Entities;

namespace OrderService.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct = default);
    Task AddAsync(Order? order, CancellationToken ct = default);
    Task UpdateAsync(Order? order, CancellationToken ct = default);
    Task<IReadOnlyList<Order>> ListByUserAsync(Guid userId, CancellationToken ct = default);
}