using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API;
using Application.Dtos.AccountDtos;
using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        public static LoginRequest AdminLogin()
        {
            return new LoginRequest()
            {
                Email = "admin@test.com",
                Password = "Pa$$w0rd"
            };
        }
        public static LoginRequest MemberLogin()
        {
            return new LoginRequest()
            {
                Email = "bob@test.com",
                Password = "Pa$$w0rd"
            };
        }
        public static LoginRequest InvalidLogin()
        {
            return new LoginRequest()
            {
                Email = "INVALID",
                Password = "INVALID"
            };
        }
        public async Task<HttpResponseMessage> GetLoginResponse(LoginRequest loginRequest)
        {
            return await Client.PostAsync("api/auth/login",
                new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, MediaTypeNames.Application.Json));
        }
        public static async Task<string> GetLoginResponseContent(HttpResponseMessage loginResponse)
        {
            return await loginResponse.Content.ReadAsStringAsync();
        }
        public static LoginResult GetLoginResult(string loginResponseContent)
        {
            return JsonSerializer.Deserialize<LoginResult>(loginResponseContent);
        }

        public async Task<AuthenticationHeaderValue> SetAuthenticationHeaderValue(LoginRequest loginRequest)
        {
            var loginResponse = await GetLoginResponse(loginRequest);
            var loginResponseContent = await GetLoginResponseContent(loginResponse);
            var loginResult = GetLoginResult(loginResponseContent);

            return new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);
        }
    }
}
