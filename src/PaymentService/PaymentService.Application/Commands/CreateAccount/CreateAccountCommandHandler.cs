using MediatR;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Application.Events.IntegrationEvents;
using PaymentService.Application.Services;

namespace PaymentService.Application.Commands.CreateAccount;

public class CreateAccountCommandHandler
    : IRequestHandler<CreateAccountCommand, CreateAccountResult>
{
    private readonly IAccountRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IIntegrationEventPublisher _publisher;

    public CreateAccountCommandHandler(
        IAccountRepository repo,
        IUnitOfWork uow,
        IIntegrationEventPublisher publisher)
    {
        _repo = repo;
        _uow = uow;
        _publisher = publisher;
    }

    public async Task<CreateAccountResult> Handle(
        CreateAccountCommand cmd,
        CancellationToken ct)
    {
        var existing = await _repo.GetByUserIdAsync(cmd.UserId, ct);
        if (existing != null)
            throw new ApplicationException($"Account for user {cmd.UserId} already exists.");

        var account = new Account(cmd.UserId, cmd.Balance, cmd.Currency); 
        await _repo.AddAsync(account, ct);

        var ie = new AccountCreatedIntegrationEvent(account.Id.Value, account.UserId);
        await _publisher.PublishAsync(ie, ct);

        await _uow.SaveChangesAsync(ct);

        return new CreateAccountResult(
            account.Id.Value,
            account.UserId,
            account.Balance.Amount,
            account.Balance.Currency);
    }
}