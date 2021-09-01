using Application.Dtos.AccountDtos;
using Application.Dtos.BasketDtos;
using Application.Dtos.OrderDtos;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_tests.Controllers.OrderControllers
{
    class OrderController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private const string baseUrl = "api/orders/";
        private const string basketUrl = "api/basket/";

        [SetUp()]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
        }

        [Test, Order(1)]
        public async Task HTTPGET_Return401UnAuthorized_GetOrdersForUser_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Test, Order(2)]
        public async Task HTTPGET_Return401UnAuthorized_CreateOrder_Test()
        {
            var response = await _httpClient.PostAsync(baseUrl, null);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Test, Order(3)]
        public async Task HTTPGET_Return401UnAuthorized_GetOrderByIdForUser_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl + "1");

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test, Order(4)]
        public async Task HTTPGET_Return200OK_GetOrdersForUser_Test()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(5)]
        public async Task HTTPGET_ReturnReturn200OK_CreateOrder_Test()
        {
            CustomerBasketDto basket = new()
            {
                Id = "basket1",
                Items = new List<BasketItemDto>()
                {
                    new BasketItemDto()
                    {
                    Id = 4,
                    ProductName = ".NET Black &amp; White Mug",
                    Price = 82,
                    Quantity = 32,
                    PictureUrl = "https://localhost:5001/images/products/2.png",
                    Brand = ".NET",
                    Type = "USB Memory Stick"
                    }
                }
            };

            await _httpClient.PutAsync(
            basketUrl + basket.Id,
            new StringContent(JsonSerializer.Serialize(basket),
            Encoding.UTF8, MediaTypeNames.Application.Json)
            );

            _httpClient.DefaultRequestHeaders.Authorization =
                await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            OrderDto orderDto = new()
            {
                BasketId = "basket1",
                DeliveryMethodId = 1,
                ShipToAddress = new AddressDto()
                {
                    FirstName = "Bob",
                    LastName = "Bobbity",
                    Street = "10 The Street",
                    City = "New York",
                    State = "NY",
                    ZipCode = "90250"
                }
            };

            var response = await _httpClient.PostAsync(
                baseUrl,
                new StringContent(JsonSerializer.Serialize(orderDto),
                Encoding.UTF8, MediaTypeNames.Application.Json)
                );

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OrderToReturnDto>(responseContent);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(result.OrderItems.Count == 1 && orderDto.ShipToAddress.FirstName == result.ShipToAddress.FirstName);
        }
        [Test, Order(6)]
        public async Task HTTPGET_ReturnReturn200OK_GetOrderByIdForUser_Test()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var response = await _httpClient.GetAsync(baseUrl + "1");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(7)]
        public async Task HTTPGET_ReturnNotFound404_GetOrderByIdForUser_Test()
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var response = await _httpClient.GetAsync(baseUrl + "0");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

    }
}
