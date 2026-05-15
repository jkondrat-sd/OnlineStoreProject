using APIGateway.API.DTOs;

namespace APIGateway.API.Clients;

public class PaymentServiceClient : IPaymentServiceClient
{
    private readonly HttpClient _http;
    private readonly ILogger<PaymentServiceClient> _logger;

    public PaymentServiceClient(HttpClient http, ILogger<PaymentServiceClient> logger)
    {
        _http = http ?? throw new ArgumentNullException(nameof(http));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CreateAccountResponse?> CreateAccountAsync(CreateAccountServiceRequest req)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync("/api/accounts", req);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<CreateAccountResponse>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to create account for request {@Request}", req);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating account for request {@Request}", req);
            return null;
        }
    }

    public async Task DepositAsync(Guid userId, DepositRequest req)
    {
        try
        {
            var resp = await _http.PostAsJsonAsync($"/api/accounts/{userId}/deposit", req);
            resp.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to deposit for user {UserId} with request {@Request}", userId, req);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while depositing for user {UserId} with request {@Request}", userId,
                req);
            throw;
        }
    }

    public async Task<BalanceResponse?> GetBalanceAsync(Guid userId)
    {
        try
        {
            var resp = await _http.GetAsync($"/api/accounts/{userId}/balance");
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadFromJsonAsync<BalanceResponse>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to retrieve balance for user {UserId}", userId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving balance for user {UserId}", userId);
            return null;
        }
    }
}