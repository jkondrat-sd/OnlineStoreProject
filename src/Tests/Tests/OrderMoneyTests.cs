using OrderService.Domain.Exceptions;
using OrderService.Domain.ValueObjects;

namespace Tests;

public class OrderMoneyTests
{
    [Fact]
    public void Add_SameCurrency_ReturnsCorrectSum()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");
        var result = money1.Add(money2);
        Assert.Equal(150m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Add_DifferentCurrency_ThrowsDomainException()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");
        Assert.Throws<DomainException>(() => money1.Add(money2));
    }

    [Fact]
    public void Subtract_SameCurrency_SufficientFunds_ReturnsCorrectDifference()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "USD");
        var result = money1.Subtract(money2);
        Assert.Equal(50m, result.Amount);
        Assert.Equal("USD", result.Currency);
    }

    [Fact]
    public void Subtract_SameCurrency_InsufficientFunds_ThrowsDomainException()
    {
        var money1 = new Money(50m, "USD");
        var money2 = new Money(100m, "USD");
        Assert.Throws<DomainException>(() => money1.Subtract(money2));
    }

    [Fact]
    public void Subtract_DifferentCurrency_ThrowsDomainException()
    {
        var money1 = new Money(100m, "USD");
        var money2 = new Money(50m, "EUR");
        Assert.Throws<DomainException>(() => money1.Subtract(money2));
    }
}