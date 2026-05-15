namespace PaymentService.Application.Commands.CreateAccount;

public record CreateAccountResult(Guid AccountId, Guid UserId, decimal Balance, string Currency);