using Application.Dtos.AccountDtos;
using Application.Dtos.BasketDtos;
using Application.Dtos.OrderDtos;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace API_tests.Controllers.OrderControllers
{
    public class PaymentsController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;
        private const string baseUrl = "api/payments";

        [SetUp()]
        public void SetUp()
        {
            _serviceProvider = _testHostFixture.ServiceProvider;
            _httpClient = _testHostFixture.Client;
        }

        [Test, Order(1)]
        public async Task HTTPGET_Return401UnAuthorized_GetPublishableKey_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test, Order(2)]
        public async Task HTTPGET_Return200OK_GetPublishableKey_Test()
        {
            var config = _serviceProvider.GetRequiredService<IConfiguration>();

            _httpClient.DefaultRequestHeaders.Authorization =
                await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var response = await _httpClient.GetAsync(baseUrl);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<string>(responseContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(config.GetSection("StripeSettings:PublishableKey").Value, result);
        }
    }
}