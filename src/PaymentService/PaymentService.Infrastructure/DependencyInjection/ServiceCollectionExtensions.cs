using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PaymentService.Application.Commands.CreateAccount;
using PaymentService.Application.Services;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Messaging;
using PaymentService.Infrastructure.Outbox;
using PaymentService.Infrastructure.Persistence;
using PaymentService.Infrastructure.Repositories;
using PaymentService.Infrastructure.EventPublishing;

namespace PaymentService.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(CreateAccountCommand).Assembly
            );
        });

        services.AddDbContext<PaymentDbContext>(opts =>
            opts.UseNpgsql(configuration.GetConnectionString("PaymentDatabase")));

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<KafkaSettings>(configuration.GetSection("KafkaSettings"));
        services.AddSingleton(res => res.GetRequiredService<IOptions<KafkaSettings>>().Value);
            

        services.AddSingleton<IKafkaProducer, KafkaProducer>();
            
        services.AddSingleton<KafkaTopicCreator>();

        services.AddHostedService<OrderCreatedConsumerHostedService>();
        services.AddHostedService<OutboxDispatcherHostedService>();

        services.AddScoped<IIntegrationEventPublisher, OutboxIntegrationEventPublisher>();
        services.AddScoped<IOutboxDispatcher, OutboxDispatcher>();


        return services;
    }
}