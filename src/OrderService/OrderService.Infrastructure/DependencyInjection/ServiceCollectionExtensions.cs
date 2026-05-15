using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Services;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Entities;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Outbox;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.EventPublishing;

namespace OrderService.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderServiceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<OrderDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("OrderDatabase")));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(OutboxMessage).Assembly);
        });

        var kafkaSection = configuration.GetSection("KafkaSettings");
        services.Configure<KafkaSettings>(kafkaSection);
        var kafkaSettings = kafkaSection.Get<KafkaSettings>();
        services.AddSingleton(kafkaSettings);
        
        services.AddSingleton<KafkaTopicCreator>();
        
        
        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        services.AddScoped<IIntegrationEventPublisher, OutboxIntegrationEventPublisher>();
        services.AddScoped<IOutboxDispatcher, OutboxDispatcher>();
        services.AddHostedService<OutboxDispatcherHostedService>();

        services.AddHostedService<PaymentResultConsumerHostedService>();

        return services;
    }
}