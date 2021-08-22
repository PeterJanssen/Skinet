using System;
using System.Net.Http;
using API;
using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence.Contexts;
using Persistence.Seeds;

namespace API_tests
{
    class TestHostFixture : IDisposable
    {
        public HttpClient Client { get; }
        public IServiceProvider ServiceProvider { get; }

        public TestHostFixture()
        {
            IHostBuilder builder = Program.CreateHostBuilder(null)
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseEnvironment("Test");
                });

            var host = builder.Start();
            ServiceProvider = host.Services;

            using (var scope = ServiceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    var appDbContext = services.GetRequiredService<StoreContext>();

                    appDbContext.Database.EnsureDeleted();
                    appDbContext.Database.Migrate();

                    StoreContextSeed.SeedAsync(appDbContext, loggerFactory).GetAwaiter().GetResult();

                    var userManager = services.GetRequiredService<UserManager<AppUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
                    StoreContextUsersSeed.SeedUsersAsync(userManager, roleManager).GetAwaiter().GetResult();
                }
                catch (Exception exception)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(exception, "An error occurred during migration");
                }
            }

            Client = host.GetTestClient();
            Console.WriteLine("TEST Host Started.");
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
