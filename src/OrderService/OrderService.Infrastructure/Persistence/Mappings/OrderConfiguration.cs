using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence.Mappings;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("orders");
        b.HasKey(o => o.Id);
        b.Property(o => o.UserId).IsRequired();
        b.Property(o => o.Status).IsRequired();
        b.Property(o => o.CreatedAt).IsRequired();
        b.OwnsMany(o => o.Items, ib =>
        {
            ib.ToTable("order_items");
            ib.WithOwner().HasForeignKey("OrderId");
            ib.HasKey("ProductId", "OrderId");
            ib.Property(i => i.Quantity).IsRequired();
            ib.OwnsOne(i => i.Price, pb =>
            {
                pb.Property(p => p.Amount).HasColumnName("Price");
                pb.Property(p => p.Currency).HasColumnName("Currency");
            });
        });
    }
}