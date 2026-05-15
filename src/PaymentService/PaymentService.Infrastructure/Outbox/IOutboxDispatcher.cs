namespace PaymentService.Infrastructure.Outbox;

public interface IOutboxDispatcher
{
    Task DispatchPendingAsync(CancellationToken ct = default);
}