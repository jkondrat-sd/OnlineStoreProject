using PaymentService.Domain.Exceptions;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Entities;

public class Account
{
    public AccountId Id { get; set; }
    public Guid UserId { get; private set; }
    public Money Balance { get; private set; }

    private Account() { }

    public Account(Guid userId, decimal initialAmount, string currency)
    {
        Id = new AccountId(Guid.NewGuid());
        UserId = userId;
        Balance = new Money(initialAmount, currency);
    }

    public void Deposit(Money amount)
    {
        if (amount.Amount <= 0)
            throw new DomainException("Deposit amount must be greater than zero");

        Balance = Balance.Add(amount);
    }

    public void Debit(Money amount)
    {
        if (amount.Amount <= 0)
            throw new DomainException("Debit amount must be greater than zero");

        Balance = Balance.Subtract(amount);

    }
}