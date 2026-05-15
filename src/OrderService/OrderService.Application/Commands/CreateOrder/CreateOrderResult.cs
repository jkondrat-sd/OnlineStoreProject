namespace OrderService.Application.Commands.CreateOrder;

public record CreateOrderResult(
    Guid OrderId,
    string Status
);