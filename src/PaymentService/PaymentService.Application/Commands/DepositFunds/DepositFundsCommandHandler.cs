using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Domain.Interfaces;
using PaymentService.Domain.ValueObjects;
using PaymentService.Application.Events.IntegrationEvents;
using PaymentService.Application.Services;

namespace PaymentService.Application.Commands.DepositFunds;

public class DepositFundsCommandHandler : IRequestHandler<DepositFundsCommand, Unit>
{
    private readonly IAccountRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IIntegrationEventPublisher _publisher;
    private readonly ILogger<DepositFundsCommandHandler> _logger;

    public DepositFundsCommandHandler(
        IAccountRepository repo,
        IUnitOfWork uow,
        IIntegrationEventPublisher publisher,
        ILogger<DepositFundsCommandHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Unit> Handle(DepositFundsCommand cmd, CancellationToken ct)
    {
        var account = await _repo.GetByUserIdAsync(cmd.UserId, ct)
                      ?? throw new ApplicationException("Account not found");

        var money = new Money(cmd.Amount, cmd.Currency);
        _logger.LogInformation("Before deposit: Balance={Balance}", account.Balance.Amount);
        account.Deposit(money);
        _logger.LogInformation("After deposit: Balance={Balance}", account.Balance.Amount);
        await _repo.UpdateAsync(account, ct);
        _logger.LogInformation("After UpdateAsync: Balance={Balance}", account.Balance.Amount);

        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes");
            throw;
        }

        var ie = new FundsDepositedIntegrationEvent(account.Id.Value, money);
        await _publisher.PublishAsync(ie, ct);

        return Unit.Value;
    }
}