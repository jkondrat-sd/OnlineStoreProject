using MediatR;
using PaymentService.Domain.Interfaces;

namespace PaymentService.Application.Queries.GetBalance;

public class GetBalanceQueryHandler
    : IRequestHandler<GetBalanceQuery, BalanceDto>
{
    private readonly IAccountRepository _repo;
    public GetBalanceQueryHandler(IAccountRepository repo)
        => _repo = repo;

    public async Task<BalanceDto> Handle(
        GetBalanceQuery q,
        CancellationToken ct)
    {
        var account = await _repo.GetByUserIdAsync(q.UserId, ct)
                      ?? throw new ApplicationException("Account not found");
        return new BalanceDto(
            account.Balance.Amount,
            account.Balance.Currency);
    }
}