using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Queries.GetOrderById;

public class GetOrderByIdQueryHandler
    : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _orderRepo;
    public GetOrderByIdQueryHandler(IOrderRepository orderRepo)
        => _orderRepo = orderRepo;

    public async Task<OrderDto> Handle(GetOrderByIdQuery q, CancellationToken ct)
    {
        var o = await _orderRepo.GetByIdAsync(q.OrderId, ct)
                ?? throw new ApplicationException("Order not found");

        return new OrderDto {
            Id = o.Id,
            UserId = o.UserId,
            TotalAmount = o.TotalAmount.Amount,
            Currency = o.TotalAmount.Currency,
            Status = o.Status.ToString(),
            Items = o.Items.Select(x => new OrderItemDto {
                ProductId = x.ProductId,
                Quantity  = x.Quantity,
                Price     = x.Price.Amount,
                Currency  = x.Price.Currency
            }).ToList(),
            CreatedAt = o.CreatedAt
        };
    }

}