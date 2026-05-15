using MediatR;

namespace OrderService.Application.Queries.GetOrdersByUser;

public record GetOrdersByUserQuery(Guid UserId) : IRequest<IReadOnlyList<OrderSummaryDto>>;