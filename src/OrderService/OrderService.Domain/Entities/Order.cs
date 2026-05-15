using OrderService.Domain.ValueObjects;
using OrderService.Domain.Exceptions;

namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Money TotalAmount 
    {
        get
        {
            if (_items.Count == 0) 
                return new Money(0);

            decimal total = _items.Sum(i => i.TotalPrice.Amount);
            return new Money(total, _items.First().TotalPrice.Currency);
        }
    }
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() { }

    public Order(Guid userId, IEnumerable<OrderItem> items)
    {
        if (items == null || !items.Any())
            throw new DomainException("Must provide at least one item");

        var distinctCurrencies = items.Select(i => i.TotalPrice.Currency).Distinct().Count();
        if (distinctCurrencies > 1)
            throw new DomainException("All order items must have the same currency");

        Id = Guid.NewGuid();
        UserId = userId;
        _items.AddRange(items);
        Status = OrderStatus.New;
        CreatedAt = DateTime.UtcNow;

    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.New && Status != OrderStatus.Pending)
            throw new DomainException($"Cannot mark order paid because order status is '{Status}'");

        Status = OrderStatus.Paid;
    }

    public void MarkCancelled(string reason)
    {
        if (Status == OrderStatus.Paid)
            throw new DomainException("Cannot mark order cancelled because order status is 'Paid'");

        Status = OrderStatus.Cancelled;
    }

}