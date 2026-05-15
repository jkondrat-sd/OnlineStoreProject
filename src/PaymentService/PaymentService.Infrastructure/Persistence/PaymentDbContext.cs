using Microsoft.EntityFrameworkCore;
using PaymentService.Domain.Entities;
using PaymentService.Infrastructure.Entities;
using PaymentService.Infrastructure.Persistence.Mapping;

namespace PaymentService.Infrastructure.Persistence;

public class PaymentDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

    public PaymentDbContext(DbContextOptions<PaymentDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.Entity<Account>().ToTable("accounts");
        modelBuilder.Entity<OutboxMessage>().ToTable("outbox_messages");
        modelBuilder.Entity<ProcessedEvent>()
            .ToTable("processed_events")
            .HasKey(e => e.OrderId);
        base.OnModelCreating(modelBuilder);
    }
}