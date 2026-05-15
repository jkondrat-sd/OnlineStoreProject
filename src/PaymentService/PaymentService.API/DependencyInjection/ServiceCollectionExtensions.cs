using PaymentService.API.Filters;
using PaymentService.Infrastructure.DependencyInjection; 

namespace PaymentService.API.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPaymentServiceApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPaymentService(configuration);

        services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            })
            .AddJsonOptions(o =>
                o.JsonSerializerOptions.PropertyNamingPolicy = null);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();


        return services;
    }
}