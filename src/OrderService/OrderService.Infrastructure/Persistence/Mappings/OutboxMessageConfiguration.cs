using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Infrastructure.Entities;

namespace OrderService.Infrastructure.Persistence.Mappings;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> b)
    {
        b.ToTable("outbox_messages");
        b.HasKey(o => o.Id);
        b.Property(o => o.OccurredOn).IsRequired();
        b.Property(o => o.EventType).HasMaxLength(200).IsRequired();
        b.Property(o => o.Payload).IsRequired();
        b.Property(o => o.SentOn);
        b.HasIndex(o => new { o.SentOn, o.OccurredOn });
    }
}