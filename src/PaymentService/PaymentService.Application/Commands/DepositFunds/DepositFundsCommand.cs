using MediatR;

namespace PaymentService.Application.Commands.DepositFunds;

public record DepositFundsCommand(Guid UserId, decimal Amount, string Currency)
    : IRequest<Unit>;