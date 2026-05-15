using APIGateway.API.Clients;
using APIGateway.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IPaymentServiceClient _payments;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IPaymentServiceClient payments, ILogger<AccountsController> logger)
    {
        _payments = payments;
        _logger = logger;
    }
    
    [HttpPost("create-account/{userId:guid}")]
    public async Task<ActionResult<CreateAccountResponse>> Create(
        [FromBody] CreateAccountRequest request, Guid userId) 
    {
        try
        {
            var serviceRequest = new CreateAccountServiceRequest
            {
                UserId = userId,
                Balance = request.Balance,
                Currency = request.Currency
            };
        
            var res = await _payments.CreateAccountAsync(serviceRequest);
            if (res == null)
            {
                _logger.LogWarning("Could not create account");
                return BadRequest("Could not create account");
            }
            return CreatedAtAction(nameof(GetBalance), new { userId = res.UserId }, res);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Creating account failed");
        }
    }
        
    [HttpPost("create-account")]
    public async Task<ActionResult<CreateAccountResponse>> Create(
        [FromBody] CreateAccountRequest request) 
    {
        try
        {
            var serviceRequest = new CreateAccountServiceRequest
            {
                UserId = Guid.NewGuid(),
                Balance = request.Balance,
                Currency = request.Currency
            };
        
            var res = await _payments.CreateAccountAsync(serviceRequest);
            if (res == null)
            {
                _logger.LogWarning("Could not create account");
                return BadRequest("Could not create account");
            }
            return CreatedAtAction(nameof(GetBalance), new { userId = res.UserId }, res);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Creating account failed");
        }
    }

    [HttpPost("{userId:guid}/deposit")]
    public async Task<IActionResult> Deposit(Guid userId, [FromBody] DepositRequest req)
    {
        try
        {
            await _payments.DepositAsync(userId, req);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Depositing account failed");
        }
    }

    [HttpGet("{userId:guid}/balance")]
    public async Task<ActionResult<BalanceResponse>> GetBalance(Guid userId)
    {
        try
        {
            var res = await _payments.GetBalanceAsync(userId);
            if (res == null)
            {
                return NotFound("Balance not found");
            }
            return Ok(res);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Balance retrieval failed");
        }
    }
}