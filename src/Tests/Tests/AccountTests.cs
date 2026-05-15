using PaymentService.Domain.Entities;
using PaymentService.Domain.Exceptions;
using PaymentService.Domain.ValueObjects;

namespace Tests;

public class AccountTests
{
    [Fact]
    public void Deposit_PositiveAmount_IncreasesBalance()
    {
        var account = new Account(Guid.NewGuid(), 100m, "USD");
        var depositAmount = new Money(50m, "USD");
        account.Deposit(depositAmount);
        Assert.Equal(150m, account.Balance.Amount);
        Assert.Equal("USD", account.Balance.Currency);
    }

    [Fact]
    public void Deposit_ZeroAmount_ThrowsDomainException()
    {
        var account = new Account(Guid.NewGuid(), 100m, "USD");
        var zeroAmount = new Money(0m, "USD");
        Assert.Throws<DomainException>(() => account.Deposit(zeroAmount));
    }

    [Fact]
    public void Deposit_DifferentCurrency_ThrowsDomainException()
    {
        var account = new Account(Guid.NewGuid(), 100m, "USD");
        var depositAmount = new Money(50m, "EUR");
        Assert.Throws<DomainException>(() => account.Deposit(depositAmount));
    }

    [Fact]
    public void Debit_PositiveAmount_SufficientFunds_DecreasesBalance()
    {
        var account = new Account(Guid.NewGuid(), 100m, "USD");
        var debitAmount = new Money(50m, "USD");
        account.Debit(debitAmount);
        Assert.Equal(50m, account.Balance.Amount);
        Assert.Equal("USD", account.Balance.Currency);
    }

    [Fact]
    public void Debit_PositiveAmount_InsufficientFunds_ThrowsDomainException()
    {
        var account = new Account(Guid.NewGuid(), 50m, "USD");
        var debitAmount = new Money(100m, "USD");
        Assert.Throws<DomainException>(() => account.Debit(debitAmount));
    }

    [Fact]
    public void Debit_ZeroAmount_ThrowsDomainException()
    {
        var account = new Account(Guid.NewGuid(), 100m, "USD");
        var zeroAmount = new Money(0m, "USD");
        Assert.Throws<DomainException>(() => account.Debit(zeroAmount));
    }

    [Fact]
    public void Debit_DifferentCurrency_ThrowsDomainException()
    {
        var account = new Account(Guid.NewGuid(), 100m, "USD");
        var debitAmount = new Money(50m, "EUR");
        Assert.Throws<DomainException>(() => account.Debit(debitAmount));
    }
}