using MediatR;

namespace OrderService.Application.Commands.ProcessPaymentResult;

public record ProcessPaymentResultCommand(
    Guid OrderId,
    bool Success,
    string FailureReason
) : IRequest<Unit>;