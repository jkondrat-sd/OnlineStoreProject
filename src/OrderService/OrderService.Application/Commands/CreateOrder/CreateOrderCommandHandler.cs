using MediatR;
using OrderService.Application.Events.IntegrationEvents;
using OrderService.Application.Services;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Domain.ValueObjects;

namespace OrderService.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler
    : IRequestHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderRepository _orderRepo;
    private readonly IUnitOfWork _uow;
    private readonly IIntegrationEventPublisher _eventPublisher;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepo,
        IUnitOfWork uow,
        IIntegrationEventPublisher eventPublisher)
    {
        _orderRepo = orderRepo;
        _uow = uow;
        _eventPublisher = eventPublisher;
    }

    public async Task<CreateOrderResult> Handle(
        CreateOrderCommand cmd,
        CancellationToken ct)
    {
        var items = cmd.Items
            .Select(i => new OrderItem(
                i.ProductId,
                i.Quantity,
                new Money(i.Price, i.Currency)))
            .ToList();

        var order = new Order(cmd.UserId, items);

        await _orderRepo.AddAsync(order, ct);

        var ievent = new OrderCreatedIntegrationEvent(
            order.Id,
            order.UserId,
            order.TotalAmount.Amount,
            order.TotalAmount.Currency);

        await _eventPublisher.PublishAsync(ievent, ct);

        await _uow.SaveChangesAsync(ct);

        return new CreateOrderResult(order.Id, order.Status.ToString());
    }
}