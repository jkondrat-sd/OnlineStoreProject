using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Domain.Interfaces;
using PaymentService.Application.Events.IntegrationEvents;
using PaymentService.Application.Services;
using PaymentService.Domain.Exceptions;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Application.Commands.ProcessOrderPayment;

public class ProcessOrderPaymentCommandHandler
    : IRequestHandler<ProcessOrderPaymentCommand, Unit>
{
    private readonly IAccountRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly IIntegrationEventPublisher _publisher;
    private readonly ILogger<ProcessOrderPaymentCommandHandler> _logger;

    public ProcessOrderPaymentCommandHandler(
        IAccountRepository repo,
        IUnitOfWork uow,
        IIntegrationEventPublisher publisher,
        ILogger<ProcessOrderPaymentCommandHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        ProcessOrderPaymentCommand cmd,
        CancellationToken ct)
    {
        _logger.LogInformation("Starting payment processing: OrderId={OrderId}, UserId={UserId}, Amount={Amount}{Currency}",
            cmd.OrderId, cmd.UserId, cmd.Amount, cmd.Currency);

        var account = await _repo.GetByUserIdAsync(cmd.UserId, ct)
                      ?? throw new ApplicationException($"Account for UserId {cmd.UserId} not found");
        _logger.LogInformation("Account parameters {Amount}{Currency}", account.Balance.Amount, account.Balance.Currency);

        var money = new Money(cmd.Amount, cmd.Currency);

        try
        {
            _logger.LogInformation("money {Money} vs account {Account}", money.Amount, account.Balance.Amount);
            account.Debit(money);
            await _repo.UpdateAsync(account, ct);
            _logger.LogInformation("Debited {Amount}{Currency} from AccountId={AccountId}", 
                money.Amount, money.Currency, account.Id.Value);

            var successEvent = new PaymentSucceededIntegrationEvent(
                cmd.OrderId,
                account.Id.Value,
                money.Amount,
                money.Currency
            );
            await _publisher.PublishAsync(successEvent, ct);

            _logger.LogInformation("Published PaymentSucceededIntegrationEvent for OrderId={OrderId}", cmd.OrderId);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning("Payment failed for OrderId={OrderId}, reason: {Reason}", cmd.OrderId, ex.Message);

            var failEvent = new PaymentFailedIntegrationEvent(
                cmd.OrderId,
                account.Id.Value,
                money.Amount,
                money.Currency,
                ex.Message
            );
            await _publisher.PublishAsync(failEvent, ct);

            _logger.LogInformation("Published PaymentFailedIntegrationEvent for OrderId={OrderId}", cmd.OrderId);
        }
        
        try
        {
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes");
            throw;
        }
        _logger.LogInformation("Completed processing of OrderId={OrderId}", cmd.OrderId);

        return Unit.Value;
    }
}
