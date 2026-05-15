using OrderService.API.Filters;
using OrderService.Infrastructure.DependencyInjection;

namespace OrderService.API.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderServiceApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOrderServiceInfrastructure(configuration);

        services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            })
            .AddJsonOptions(opts =>
                opts.JsonSerializerOptions.PropertyNamingPolicy = null);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}