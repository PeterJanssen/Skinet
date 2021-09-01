using Application.Dtos.BasketDtos;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_tests.Controllers.BasketControllers
{
    class BasketController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private const string baseUrl = "api/basket/";

        [SetUp()]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
        }

        [Test, Order(1)]
        public async Task HTTPPUT_Return200OK_UpdateBasket()
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

            var response = await _httpClient.PutAsync(
            baseUrl + basket.Id,
            new StringContent(JsonSerializer.Serialize(basket),
            Encoding.UTF8, MediaTypeNames.Application.Json)
            );

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CustomerBasketDto>(responseContent);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(basket.Id == result.Id && basket.Items.Count == 1);
        }
        [Test, Order(2)]
        public async Task HTTPGET_Return200OK_GetBasketById_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl + "basket1");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(3)]
        public async Task HTTPDelete_Return200OK_DeleteBasketAsync_Test()
        {
            var response = await _httpClient.DeleteAsync(baseUrl + "basket1");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(4)]
        public async Task HTTPDelete_Return404NotFound_DeleteBasketAsync_Test()
        {
            var response = await _httpClient.DeleteAsync(baseUrl + "basket1");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
