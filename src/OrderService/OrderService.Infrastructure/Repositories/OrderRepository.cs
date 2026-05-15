using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _ctx;
    public OrderRepository(OrderDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Order? order, CancellationToken ct = default)
        => await _ctx.Orders.AddAsync(order, ct);

    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct = default)
        => await _ctx.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);

    public async Task<IReadOnlyList<Order>> ListByUserAsync(Guid userId, CancellationToken ct = default)
        => await _ctx.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);

    public Task UpdateAsync(Order? order, CancellationToken ct = default)
    {
        _ctx.Orders.Update(order);
        return Task.CompletedTask;
    }
}