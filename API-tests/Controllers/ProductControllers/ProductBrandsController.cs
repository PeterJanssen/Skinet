using Application.Dtos.ProductDtos;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_tests.Controllers.ProductControllers
{
    class ProductBrandsController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private const string baseUrl = "api/products/brands";

        [SetUp()]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
        }

        [Test, Order(1)]
        public async Task HTTPGET_Return200OK_GetProductBrands_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(2)]
        public async Task HTTPGET_ReturnsAllBrands_GetProductBrands_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            var productBrandsResponseContent = await response.Content.ReadAsStringAsync();
            var productBrandResult = JsonSerializer.Deserialize<IReadOnlyList<ProductBrandDto>>(productBrandsResponseContent);

            Assert.IsNotNull(productBrandResult);
            Assert.That(productBrandResult.Count, Is.EqualTo(6));
        }
    }
}
