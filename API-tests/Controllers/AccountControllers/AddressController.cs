using Application.Core.Services.Implementations.Identity;
using Application.Core.Services.Interfaces.Identity.JWT;
using Application.Dtos.AccountDtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mime;


namespace API_tests.Controllers.AccountControllers
{
    class AddressController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;
        private const string baseUrl = "api/account/address";
        private const string authUrl = "api/auth";

        [SetUp()]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
            _serviceProvider = _testHostFixture.ServiceProvider;
        }

        [Test, Order(1)]
        public async Task HTTPGET_Return401UnAuthorized_GetUserAddress_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test, Order(2)]
        public async Task HTTPPUT_Return401Unauthorized_UpdateUserAddress_Test()
        {
            var updatedAddress = new AddressDto()
            {
                City = "Miami",
                FirstName = "Bobbette",
                LastName = "Bobbitien",
                State = "Miami state",
                Street = "Miami street",
                ZipCode = "1234"
            };

            var response = await _httpClient.PutAsync(baseUrl,
                new StringContent(JsonSerializer.Serialize(updatedAddress), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test, Order(3)]
        public async Task HTTPGET_Return200OK_ReturnsUserAdress_GetUserAddress_Test()
        {
            var credentials = new LoginRequest
            {
                Email = "bob@test.com",
                Password = "Pa$$w0rd"
            };

            var loginResponse = await _httpClient.PostAsync(authUrl + "/login",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);

            var response = await _httpClient.GetAsync(baseUrl);

            var addressResponseContent = await response.Content.ReadAsStringAsync();
            var address = JsonSerializer.Deserialize<AddressDto>(addressResponseContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("New York", address.City);
            Assert.AreEqual("Bob", address.FirstName);
            Assert.AreEqual("Bobbity", address.LastName);
            Assert.AreEqual("NY", address.State);
            Assert.AreEqual("10 The Street", address.Street);
            Assert.AreEqual("90210", address.ZipCode);
        }

        [Test, Order(4)]
        public async Task HTTPPUT_Return400BadRequest_UpdateUserAddress_Test()
        {
            AddressDto updatedAddress = null;

            var response = await _httpClient.PutAsync(baseUrl,
                new StringContent(JsonSerializer.Serialize(updatedAddress), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test, Order(5)]
        public async Task HTTPPUT_Return200OK_ReturnsUserUpdatedAdress_UpdateUserAddress_Test()
        {
            var credentials = new LoginRequest
            {
                Email = "bob@test.com",
                Password = "Pa$$w0rd"
            };

            var loginResponse = await _httpClient.PostAsync(authUrl + "/login",
                new StringContent(JsonSerializer.Serialize(credentials), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await loginResponse.Content.ReadAsStringAsync();
            var loginResult = JsonSerializer.Deserialize<LoginResult>(loginResponseContent);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);

            var updatedAddress = new AddressDto()
            {
                City = "Miami",
                FirstName = "Bobbette",
                LastName = "Bobbitien",
                State = "Miami state",
                Street = "Miami street",
                ZipCode = "1234"
            };

            var response = await _httpClient.PutAsync(baseUrl,
                new StringContent(JsonSerializer.Serialize(updatedAddress), Encoding.UTF8, MediaTypeNames.Application.Json));

            var addressResponseContent = await response.Content.ReadAsStringAsync();
            var address = JsonSerializer.Deserialize<AddressDto>(addressResponseContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(updatedAddress.City, address.City);
            Assert.AreEqual(updatedAddress.FirstName, address.FirstName);
            Assert.AreEqual(updatedAddress.LastName, address.LastName);
            Assert.AreEqual(updatedAddress.State, address.State);
            Assert.AreEqual(updatedAddress.Street, address.Street);
            Assert.AreEqual(updatedAddress.ZipCode, address.ZipCode);
        }
    }
}
