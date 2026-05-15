using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateOrder;

public record CreateOrderCommand(
    Guid UserId,
    IList<CreateOrderDto> Items
) : IRequest<CreateOrderResult>;