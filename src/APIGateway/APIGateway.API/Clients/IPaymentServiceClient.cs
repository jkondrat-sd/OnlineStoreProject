using APIGateway.API.DTOs;

namespace APIGateway.API.Clients;

public interface IPaymentServiceClient
{
    Task<CreateAccountResponse?> CreateAccountAsync(CreateAccountServiceRequest req);
    Task DepositAsync(Guid userId, DepositRequest req);
    Task<BalanceResponse?> GetBalanceAsync(Guid userId);
}