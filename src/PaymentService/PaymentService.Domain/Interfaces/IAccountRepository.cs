using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task AddAsync(Account account, CancellationToken ct = default);
    Task UpdateAsync(Account account, CancellationToken ct = default);
}