using Application.Core.Paging;
using Application.Dtos.Product;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API_tests.Controllers.ProductControllers
{
    class ProductsController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private const string baseUrl = "api/products";

        [SetUp()]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
        }

        [Test, Order(1)]
        public async Task HTTPGET_Return200OK_GetProducts_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(2)]
        public async Task HTTPGET_WithoutQuery_ReturnsAllProducts_GetProducts_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            var productResponseContent = await response.Content.ReadAsStringAsync();
            var productResult = JsonSerializer.Deserialize<Pagination<ProductToReturnDto>>(productResponseContent);

            Assert.IsNotNull(productResult);
            Assert.That(productResult.Count, Is.EqualTo(18));
            Assert.IsNotNull(productResult.Data);
            Assert.That(productResult.Data.Count, Is.EqualTo(6));
            Assert.That(productResult.PageIndex, Is.EqualTo(1));
            Assert.That(productResult.PageSize, Is.EqualTo(6));
        }
        [Test, Order(3)]
        public async Task HTTPGET_WithQuery_ReturnsPaginatedData_GetProducts_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl + "?PageIndex=2&PageSize=2");

            var productResponseContent = await response.Content.ReadAsStringAsync();
            var productResult = JsonSerializer.Deserialize<Pagination<ProductToReturnDto>>(productResponseContent);

            Assert.IsNotNull(productResult);
            Assert.That(productResult.Count, Is.EqualTo(18));
            Assert.IsNotNull(productResult.Data);
            Assert.That(productResult.Data.Count, Is.EqualTo(2));
            Assert.That(productResult.PageIndex, Is.EqualTo(2));
            Assert.That(productResult.PageSize, Is.EqualTo(2));
        }
        [Test, Order(4)]
        public async Task HTTPGET_Return200OK_GetProduct()
        {
            var response = await _httpClient.GetAsync(baseUrl + "/1");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(5)]
        public async Task HTTPGET_Return404NotFound_GetProduct()
        {
            var response = await _httpClient.GetAsync(baseUrl + "/0");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Test, Order(6)]
        public async Task HTTPPOST_Return200OK_CreateProduct()
        {
            _httpClient.DefaultRequestHeaders.Authorization = await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            ProductCreateDto product = new()
            {
                Name = "Angular Speedster Board 2000",
                Description = "Lorem ipsum dolor sit",
                Price = 200,
                PictureUrl = "images/products/sb-ang1.png",
                ProductBrandId = 1,
                ProductTypeId = 1
            };

            var response = await _httpClient.PostAsync(
                baseUrl,
                new StringContent(JsonSerializer.Serialize(product),
                Encoding.UTF8, MediaTypeNames.Application.Json)
                );

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ProductToReturnDto>(responseContent);

            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsTrue(product.Name == result.Name && product.Description == result.Description);
        }
        [Test, Order(7)]
        public async Task HTTPPOST_Return400BadRequest_CreateProduct()
        {
            _httpClient.DefaultRequestHeaders.Authorization = await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            ProductCreateDto product = null;

            var response = await _httpClient.PostAsync(baseUrl,
                            new StringContent(JsonSerializer.Serialize(product),
                            Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [Test, Order(8)]
        public async Task HTTPDELETE_Return200Ok_DeleteProduct()
        {
            _httpClient.DefaultRequestHeaders.Authorization = await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var response = await _httpClient.DeleteAsync(
            baseUrl + "/1");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(9)]
        public async Task HTTPDELETE_Return404NotFound_DeleteProduct()
        {
            _httpClient.DefaultRequestHeaders.Authorization = await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var response = await _httpClient.DeleteAsync(baseUrl + "/0");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Test, Order(10)]
        public async Task HTTPPUT_Return200OK_AddProductReview()
        {
            _httpClient.DefaultRequestHeaders.Authorization = await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var productReview = new ProductReviewDto()
            {
                Rating = 1,
                ReviewText = "Test"
            };

            var response = await _httpClient.PutAsync(baseUrl + "/2/rating",
                                new StringContent(JsonSerializer.Serialize(productReview),
                                Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test, Order(11)]
        public async Task HTTPPUT_Return404NotFound_AddProductReview()
        {
            _httpClient.DefaultRequestHeaders.Authorization = await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            var productReview = new ProductReviewDto()
            {
                Rating = 1,
                ReviewText = "Test"
            };

            var response = await _httpClient.PutAsync(baseUrl + "/0/rating",
                                new StringContent(JsonSerializer.Serialize(productReview),
                                Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        [Test, Order(12)]
        public async Task HTTPPUT_Return400BadRequest_AddProductReview()
        {
            _httpClient.DefaultRequestHeaders.Authorization = await _testHostFixture.SetAuthenticationHeaderValue(TestHostFixture.AdminLogin());

            ProductReviewDto productReview = null;

            var response = await _httpClient.PutAsync(baseUrl + "/2/rating",
                                new StringContent(JsonSerializer.Serialize(productReview),
                                Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
