using System;
using System.IO;
using System.Threading.Tasks;
using Core.Entities.AccountEntities;
using Infrastructure.Data.Contexts;
using Infrastructure.Data.SeedData;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace API
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json",
                        optional: false,
                        reloadOnChange: true)
                    .AddJsonFile(string.Format("appsettings.{0}.json",
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                            ?? "Production"),
                        optional: true,
                        reloadOnChange: true)
                    .AddUserSecrets<Startup>(optional: true, reloadOnChange: true)
                    .Build();

            Log.Logger = new LoggerConfiguration()
                        .WriteTo.PostgreSQL(
                            connectionString:
                                configuration.GetConnectionString("DefaultConnection"),
                            restrictedToMinimumLevel: LogEventLevel.Error,
                            tableName: "LogErrorEvents",
                            needAutoCreateTable: true)
                        .WriteTo.File(
                                path: "Logs/Error/logErrors.txt",
                                rollingInterval: Serilog.RollingInterval.Day,
                                restrictedToMinimumLevel: LogEventLevel.Error,
                                outputTemplate: "{Timestamp} [{Level}] - Message: {Message}{NewLine}{Exception}{NewLine}{NewLine}"
                        )
                        .WriteTo.File(
                                path: "Logs/Info/logInfo.txt",
                                rollingInterval: Serilog.RollingInterval.Day,
                                restrictedToMinimumLevel: LogEventLevel.Information,
                                outputTemplate: "{Timestamp} [{Level}] - Message: {Message}{NewLine}{Exception}{NewLine}{NewLine}"
                        )
                        .WriteTo.Console()
                        .CreateLogger();

            var host = CreateHostBuilder(args).UseSerilog().Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var storeContext = services.GetRequiredService<StoreContext>();
                    await storeContext.Database.MigrateAsync();
                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                    await StoreContextSeed.SeedAsync(storeContext, loggerFactory);
                    await StoreContextUsersSeed.SeedUsersAsync(userManager, roleManager);
                }
                catch (Exception exception)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(exception, "An error occurred during migration");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
