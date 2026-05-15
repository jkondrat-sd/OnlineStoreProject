using MediatR; using OrderService.Domain.Interfaces; using OrderService.Application.Services; using OrderService.Domain.Entities; using Microsoft.Extensions.Logging;
using OrderService.Application.Events.IntegrationEvents;

namespace OrderService.Application.Commands.ProcessPaymentResult;

public class ProcessPaymentResultCommandHandler : IRequestHandler<ProcessPaymentResultCommand, Unit> {
    private readonly IOrderRepository _orderRepo;
    private readonly IUnitOfWork _uow;
    private readonly IIntegrationEventPublisher _eventPublisher;
    private readonly ILogger _logger;

    public ProcessPaymentResultCommandHandler(
        IOrderRepository orderRepo,
        IUnitOfWork uow,
        IIntegrationEventPublisher eventPublisher,
        ILogger<ProcessPaymentResultCommandHandler> logger)
    {
        _orderRepo = orderRepo;
        _uow = uow;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        ProcessPaymentResultCommand cmd,
        CancellationToken ct)
    {
        _logger.LogInformation("Processing payment result for OrderId={OrderId}, Success={Success}", cmd.OrderId, cmd.Success);

        var order = await _orderRepo.GetByIdAsync(cmd.OrderId, ct)
                    ?? throw new ApplicationException("Order not found");

        if (cmd.Success)
            order.MarkPaid();
        else
            order.MarkCancelled(cmd.FailureReason);

        await _orderRepo.UpdateAsync(order, ct);
        _logger.LogInformation("Updated order {OrderId} to status {Status}", order.Id, order.Status);

        var statusEvent = new OrderStatusChangedIntegrationEvent(
            order.Id,
            order.Status.ToString());
        await _eventPublisher.PublishAsync(statusEvent, ct);

        await _uow.SaveChangesAsync(ct);
        _logger.LogInformation("Saved changes for OrderId={OrderId}", order.Id);

        return Unit.Value;
    }

}