using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.API.DTOs;
using PaymentService.Application.Commands.CreateAccount;
using PaymentService.Application.Commands.DepositFunds;
using PaymentService.Application.Queries.GetBalance;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IMediator _mediator;
    public AccountsController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<CreateAccountResult>> CreateAccount(
        [FromBody] CreateAccountCommand cmd)
    {
        var result = await _mediator.Send(cmd);
        return CreatedAtAction(nameof(GetBalance), new { userId = result.UserId }, result);
    }

    [HttpPost("{userId:guid}/deposit")]
    public async Task<IActionResult> Deposit(
        Guid userId,
        [FromBody] DepositRequest req)
    {
        var cmd = new DepositFundsCommand(userId, req.Amount, req.Currency);
        await _mediator.Send(cmd);
        return NoContent();
    }

    [HttpGet("{userId:guid}/balance")]
    public async Task<ActionResult<BalanceDto>> GetBalance(Guid userId)
    {
        var q = new GetBalanceQuery(userId);
        var dto = await _mediator.Send(q);
        return Ok(dto);
    }
        
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok();
    }
}