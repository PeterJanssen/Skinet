using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Persistence.Contexts;

namespace API.Extensions
{
    public static class HealthCheck
    {
        public static IServiceCollection AddHealthCheckExtension(this IServiceCollection services, IConfiguration config)
        {

            services.AddHealthChecks()
                .AddDbContextCheck<StoreContext>()
                .AddRedis(config.GetConnectionString("Redis"),
                failureStatus: HealthStatus.Unhealthy,
                name: "Redis Server")
                .AddNpgSql(config.GetConnectionString("DefaultConnection"),
                healthQuery: "select 1",
                failureStatus: HealthStatus.Unhealthy,
                name: "PostGreSQL Server")
                .AddUrlGroup(new Uri("https://localhost:4200/"),
                name: "Client URL",
                failureStatus: HealthStatus.Unhealthy);

            services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(30); //time in seconds between check    
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                opt.SetApiMaxActiveRequests(5); //api requests concurrency    
                opt.AddHealthCheckEndpoint("Health Checks", "https://localhost:5001/health"); //map health check api    
            }).AddInMemoryStorage();

            return services;
        }
    }
}