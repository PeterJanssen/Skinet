using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Contexts;
using StackExchange.Redis;

namespace API.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabaseConnectionsExtension(this IServiceCollection services, IConfiguration config)
        {

            services.AddDbContext<StoreContext>
                (x => x.UseNpgsql(config.GetConnectionString("DefaultConnection")));

            services.AddSingleton<IConnectionMultiplexer>(c =>
                {
                var configuration = ConfigurationOptions.Parse(
                config.GetConnectionString("Redis"), true);

                return ConnectionMultiplexer.Connect(configuration);
                });
            return services;
        }
    }
}