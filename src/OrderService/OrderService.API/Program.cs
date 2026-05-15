using Microsoft.EntityFrameworkCore;
using OrderService.API.DependencyInjection;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Persistence;

namespace OrderService.API;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Logging.AddConsole();
        
        builder.Services.AddOrderServiceApi(builder.Configuration);
        
        var connectionString = builder.Configuration.GetConnectionString("OrderDatabase");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Missing OrderDatabase connection string");
        }

        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
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
                    if (retry == maxRetries - 1) throw;
                    await Task.Delay(5000);
                }
            }
        }
        
        using (var scope = app.Services.CreateScope())
        {
            var topicCreator = scope.ServiceProvider.GetRequiredService<KafkaTopicCreator>();
            
            try { 
                await topicCreator.CreateTopicsAsync();
            } catch (Exception ex) { 
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
        catch (Exception e)
        {
            logger.LogError(e, "Failed to run OrderService");
            throw;
        }
        
    }
}