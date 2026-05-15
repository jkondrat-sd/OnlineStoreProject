using OrderService.Domain.Exceptions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

public class OrderItem
{
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public Money Price { get; private set; }

    public OrderItem() { }

    public OrderItem(Guid productId, int quantity, Money price)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be greater than zero");
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }

    public Money TotalPrice => Price.Multiply(Quantity);
}