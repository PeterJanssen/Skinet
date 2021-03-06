using System;
using System.IO;
using System.Threading.Tasks;
using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Seeds;
using Serilog;
using Serilog.Events;

namespace API
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            try
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
                            .ReadFrom.Configuration(configuration)
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
                        await StoreContextSeed.SeedAsync(storeContext, loggerFactory);

                        var userManager = services.GetRequiredService<UserManager<AppUser>>();
                        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
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
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
