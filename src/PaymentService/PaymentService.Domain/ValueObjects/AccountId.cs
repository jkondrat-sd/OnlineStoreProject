using PaymentService.Domain.Exceptions;

namespace PaymentService.Domain.ValueObjects;

public sealed class AccountId
{
    public Guid Value { get; }

    public AccountId(Guid value)
    {
        if (value == Guid.Empty) throw new DomainException("AccountId cannot be empty");
        Value = value;
    }

    public override bool Equals(object obj)
        => obj is AccountId id && id.Value == Value;

    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString();
}