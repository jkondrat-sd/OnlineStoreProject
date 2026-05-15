using MediatR;

namespace PaymentService.Application.Commands.ProcessOrderPayment;

public record ProcessOrderPaymentCommand(
    Guid UserId,
    Guid OrderId,
    decimal Amount,
    string Currency) : IRequest<Unit>;