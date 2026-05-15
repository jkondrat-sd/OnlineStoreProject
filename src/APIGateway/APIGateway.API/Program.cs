using APIGateway.API.Clients;
using Microsoft.Extensions.Options;

namespace APIGateway.API;

class Program
{

    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Logging.AddConsole();

        builder.Services.Configure<ServiceEndpoints>(builder.Configuration.GetSection("ServiceEndpoints"));

        
        var orderServiceUrl = builder.Configuration["ServiceEndpoints:OrderService"];
        var paymentServiceUrl = builder.Configuration["ServiceEndpoints:PaymentService"];
        Console.WriteLine($"OrderService URL: {orderServiceUrl}");
        Console.WriteLine($"PaymentService URL: {paymentServiceUrl}");
        
        builder.Services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ServiceEndpoints:PaymentService"]!);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        builder.Services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["ServiceEndpoints:OrderService"]!);
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        builder.Services.AddHttpClient("OrderService", (sp, client) => 
        {
            var opts = sp.GetRequiredService<IOptions<ServiceEndpoints>>().Value;
            client.BaseAddress = new Uri(opts.OrderService);
        });

        builder.Services.AddHttpClient("PaymentService", (sp, client) => 
        {
            var opts = sp.GetRequiredService<IOptions<ServiceEndpoints>>().Value;
            client.BaseAddress = new Uri(opts.PaymentService);
        });

        builder.Services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
            p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

        var app = builder.Build();

        app.UseCors();
        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();
        app.Run();
    }
}
