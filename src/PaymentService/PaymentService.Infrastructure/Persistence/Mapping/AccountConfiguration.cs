using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentService.Domain.Entities;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Infrastructure.Persistence.Mapping;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> b)
    {
        b.ToTable("accounts");

        b.HasKey(a => a.Id);

        b.Property(a => a.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => new AccountId(value)
            )
            .ValueGeneratedOnAdd();
        
        b.Property(a => a.UserId)
            .HasColumnName("user_id")
            .IsRequired();
        
        b.OwnsOne(a => a.Balance, mb =>
        {
            mb.Property(m => m.Amount)
                .HasColumnName("balance_amount")
                .IsRequired();

            mb.Property(m => m.Currency)
                .HasColumnName("balance_currency")
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}