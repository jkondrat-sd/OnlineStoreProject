using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Entities;
using OrderService.Infrastructure.Persistence.Mappings;

namespace OrderService.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.Entity<Order>().ToTable("orders");
        modelBuilder.Entity<OutboxMessage>().ToTable("outbox_messages");
        base.OnModelCreating(modelBuilder);
    }
}