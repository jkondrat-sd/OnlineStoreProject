using Microsoft.EntityFrameworkCore;
using PaymentService.API.DependencyInjection;
using PaymentService.Infrastructure.Messaging;
using PaymentService.Infrastructure.Persistence;

namespace PaymentService.API;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddConsole();

        builder.Services.AddPaymentServiceApi(builder.Configuration);

        var connectionString = builder.Configuration.GetConnectionString("PaymentDatabase");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Missing PaymentDatabase connection string");
        }

        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
            const int maxRetries = 5;
            for (int retry = 0; retry < maxRetries; retry++)
            {
                try
                {
                    logger.LogInformation("Migration attempt ({Attempt}/{MaxRetries})", retry + 1, maxRetries);
                    await db.Database.MigrateAsync();
                    logger.LogInformation("Migration success");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Migration error on attempt {Attempt}/{MaxRetries}", retry + 1, maxRetries);
                    if (retry == maxRetries - 1)
                        throw;
                    await Task.Delay(TimeSpan.FromSeconds(5), CancellationToken.None);
                }
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var topicCreator = scope.ServiceProvider.GetRequiredService<KafkaTopicCreator>();
            try
            {
                await topicCreator.CreateTopicsAsync();
                logger.LogInformation("Kafka topics created successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create Kafka topics");
            }
        }

        app.UseRouting();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();

        try
        {
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to run PaymentService");
            throw;
        }
    }
}