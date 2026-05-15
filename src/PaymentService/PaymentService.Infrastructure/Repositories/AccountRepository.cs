using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.Infrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly PaymentDbContext _ctx;
    public AccountRepository(PaymentDbContext ctx) => _ctx = ctx;

    public async Task AddAsync(Account account, CancellationToken ct)
    {
        foreach (var dbaccount in _ctx.Accounts)
        {
            Console.WriteLine(dbaccount.UserId + " | " + dbaccount.Balance.Amount + " | " + dbaccount.Balance.Currency);
        }
        await _ctx.Accounts.AddAsync(account, ct);
    }

    public async Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        foreach (var dbaccount in _ctx.Accounts)
        {
            Console.WriteLine(dbaccount.UserId + " | " + dbaccount.Balance.Amount + " | " + dbaccount.Balance.Currency);
        }
        return await _ctx.Accounts.FirstOrDefaultAsync(a => a.UserId == userId, ct);
    }

    public Task UpdateAsync(Account account, CancellationToken ct = default)
    {
        foreach (var dbaccount in _ctx.Accounts)
        {
            Console.WriteLine(dbaccount.UserId + " | " + dbaccount.Balance.Amount + " | " + dbaccount.Balance.Currency);
        }
        _ctx.Accounts.Update(account);
        return Task.CompletedTask;
    }
}