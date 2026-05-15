using MediatR;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Queries.GetOrdersByUser;


public class GetOrdersByUserQueryHandler
    : IRequestHandler<GetOrdersByUserQuery, IReadOnlyList<OrderSummaryDto>>
{
    private readonly IOrderRepository _orderRepo;
    public GetOrdersByUserQueryHandler(IOrderRepository orderRepo)
        => _orderRepo = orderRepo;

    public async Task<IReadOnlyList<OrderSummaryDto>> Handle(
        GetOrdersByUserQuery q,
        CancellationToken ct)
    {
        var list = await _orderRepo.ListByUserAsync(q.UserId, ct);

        return list.Select(o => new OrderSummaryDto {
            Id = o.Id,
            TotalAmount = o.TotalAmount.Amount,
            Status = o.Status.ToString(),
            CreatedAt = o.CreatedAt
        }).ToList();
    }
}
