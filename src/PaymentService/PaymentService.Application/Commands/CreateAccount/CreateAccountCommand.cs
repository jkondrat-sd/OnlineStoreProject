using MediatR;

namespace PaymentService.Application.Commands.CreateAccount;

public record CreateAccountCommand(
    Guid UserId, 
    decimal Balance, 
    string Currency) : IRequest<CreateAccountResult>;