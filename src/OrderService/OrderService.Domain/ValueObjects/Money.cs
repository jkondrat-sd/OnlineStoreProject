using OrderService.Domain.Exceptions;

namespace OrderService.Domain.ValueObjects;

public sealed class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money() { }

    public Money(decimal amount, string currency = "USD")
    {
        if (amount < 0) throw new DomainException("Amount cannot be negative");
        Amount = amount;
        Currency = currency;
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        var result = Amount - other.Amount;
        if (result < 0) throw new DomainException("Not enough money");
        return new Money(result, Currency);
    }
        
    public Money Multiply(decimal quantity)
    {
        return new Money(Amount * quantity, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException("Currency does not match");
    }

    public override bool Equals(object obj)
        => obj is Money m && m.Amount == Amount && m.Currency == Currency;

    public override int GetHashCode()
        => HashCode.Combine(Amount, Currency);
}