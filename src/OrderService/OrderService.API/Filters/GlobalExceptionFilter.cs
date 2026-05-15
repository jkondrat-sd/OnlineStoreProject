using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OrderService.Domain.Exceptions;

namespace OrderService.API.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception");

        var response = new ObjectResult(new
        {
            error = context.Exception.Message
        });

        if (context.Exception is DomainException or ApplicationException)
        {
            response.StatusCode = 400;
        }
        else
        {
            response.StatusCode = 500;
            response.Value = new { error = "Unhandled exception" };
        }

        context.Result = response;
        context.ExceptionHandled = true;
    }
}