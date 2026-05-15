using MediatR;

namespace PaymentService.Application.Queries.GetBalance;

public record GetBalanceQuery(Guid UserId) : IRequest<BalanceDto>;