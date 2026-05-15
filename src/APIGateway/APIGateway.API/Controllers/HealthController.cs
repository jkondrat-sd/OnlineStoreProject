using Microsoft.AspNetCore.Mvc;

namespace APIGateway.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HealthController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var orderClient = _httpClientFactory.CreateClient("OrderService");
        var paymentClient = _httpClientFactory.CreateClient("PaymentService");

        try
        {
            var orderResponse = await orderClient.GetAsync("api/orders/health");
            var paymentResponse = await paymentClient.GetAsync("api/accounts/health");

            var status = new
            {
                OrderService = orderResponse.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                PaymentService = paymentResponse.IsSuccessStatusCode ? "Healthy" : "Unhealthy"
            };

            if (orderResponse.IsSuccessStatusCode && paymentResponse.IsSuccessStatusCode)
            {
                return Ok(new { Status = "Healthy", Services = status });
            }
            return StatusCode(500, new { Status = "Unhealthy", Services = status });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Status = "Unhealthy", Error = $"Failed to reach services: {ex.Message}" });
        }
    }
}