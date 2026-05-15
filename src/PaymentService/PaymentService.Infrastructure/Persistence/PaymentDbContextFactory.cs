using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PaymentService.Infrastructure.Persistence;

public class PaymentDbContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
{
    public PaymentDbContext CreateDbContext(string[] args)
    {
        var projectDir = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), 
                "..",
                "PaymentService.API"
                ));
        
        var config = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var options = new DbContextOptionsBuilder<PaymentDbContext>()
            .UseNpgsql(config.GetConnectionString("PaymentDatabase"))
            .Options;

        return new PaymentDbContext(options);
    }
}