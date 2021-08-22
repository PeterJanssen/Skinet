using Application.Dtos.OrderDtos;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_tests.Controllers.OrderControllers
{
    class DeliveryMethodController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private const string baseUrl = "api/orders/deliverymethods";

        [SetUp()]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
        }
        [Test, Order(1)]
        public async Task HTTPGET_Return401Unauthorized_GetDeliveryMethods_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Test, Order(2)]
        public async Task HTTPGET_Return200OK_GetDeliveryMethods_Test()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(3)]
        public async Task HTTPGET_ReturnsAllDeliveryMethods_GetDeliveryMethods_Test()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var response = await _httpClient.GetAsync(baseUrl);

            var deliveryMethodResponseContent = await response.Content.ReadAsStringAsync();
            var deliveryMethodResult = JsonSerializer.Deserialize<IReadOnlyList<DeliveryMethodDto>>(deliveryMethodResponseContent);

            Assert.IsNotNull(deliveryMethodResult);
            Assert.That(deliveryMethodResult.Count, Is.EqualTo(4));
        }
    }
}
