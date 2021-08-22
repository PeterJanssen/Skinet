using Application.Dtos.ProductDtos;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_tests.Controllers.ProductControllers
{
    class ProductTypesController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private const string baseUrl = "api/products/types";

        [SetUp()]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
        }

        [Test, Order(1)]
        public async Task HTTPGET_Return200OK_GetProductTypes_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(2)]
        public async Task HTTPGET_ReturnsAllBrands_GetProductTypes_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            var productTypeResponseContent = await response.Content.ReadAsStringAsync();
            var productTypeResult = JsonSerializer.Deserialize<IReadOnlyList<ProductTypeDto>>(productTypeResponseContent);

            Assert.IsNotNull(productTypeResult);
            Assert.That(productTypeResult.Count, Is.EqualTo(4));
        }
    }
}
