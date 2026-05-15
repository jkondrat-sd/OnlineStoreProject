using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PaymentDbContext _ctx;
    public UnitOfWork(PaymentDbContext ctx) => _ctx = ctx;
    public Task<int> SaveChangesAsync(CancellationToken ct)
        => _ctx.SaveChangesAsync(ct);
}