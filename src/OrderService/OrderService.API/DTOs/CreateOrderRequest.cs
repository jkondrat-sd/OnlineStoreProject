using OrderService.Application.DTOs;

namespace OrderService.API.DTOs;

public class CreateOrderRequest
{
    public IList<CreateOrderDto> Items { get; set; }
}