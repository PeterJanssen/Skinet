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
using Application.Core.Services.Interfaces.Identity;

namespace API_tests.Controllers.AccountControllers
{
    class AccountController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;
        private const string baseUrl = "api/account";

        [SetUp()]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
            _serviceProvider = _testHostFixture.ServiceProvider;
        }

        [Test, Order(1)]
        public async Task HTTPGET_Return401UnAuthorized_GetCurrentUser_Test()
        {
            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Test, Order(2)]
        public async Task HTTPGET_Return200OK_ReturnsCurrentUser_GetCurrentUser_Test()
        {
            var credentials = TestHostFixture.AdminLogin();

            var loginResponse = await _testHostFixture.GetLoginResponse(credentials);

            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await TestHostFixture.GetLoginResponseContent(loginResponse);
            var loginResult = TestHostFixture.GetLoginResult(loginResponseContent);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);

            var response = await _httpClient.GetAsync(baseUrl);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await TestHostFixture.GetLoginResponseContent(response);
            var result = TestHostFixture.GetLoginResult(responseContent);

            Assert.AreEqual(credentials.Email, result.Email);
            Assert.IsTrue(result.Roles.Contains(UserRoles.Admin));
        }

        [Test, Order(3)]
        public async Task HTTPPOST_Return200OK_ShouldAllowAdminImpersonateOthers_Impersonate_Test()
        {
            string userName = TestHostFixture.AdminLogin().Email;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.Admin)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var userManager = _serviceProvider.GetRequiredService<IUserService>();
            var user = await userManager.GetUser(userName);

            var accessToken = jwtAuthManager.GenerateToken(user, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

            var request = new ImpersonationRequest { UserName = "amber@test.com" };
            var response = await _httpClient.PostAsync(baseUrl + "/impersonation",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, MediaTypeNames.Application.Json));

            var responseContent = await TestHostFixture.GetLoginResponseContent(response);
            var result = TestHostFixture.GetLoginResult(responseContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(request.UserName, result.Email);
            Assert.AreEqual(userName, result.OriginalUserName);
            Assert.IsTrue(result.Roles.Contains(UserRoles.Member));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));

            var (principal, jwtSecurityToken) = jwtAuthManager.DecodeJwtToken(result.AccessToken);
            Assert.AreEqual(request.UserName, principal.Identity.Name);
            Assert.AreEqual(UserRoles.Member, principal.FindFirst(ClaimTypes.Role).Value);
            Assert.AreEqual(userName, principal.FindFirst("OriginalUserName").Value);
            Assert.IsNotNull(jwtSecurityToken);
        }

        [Test, Order(4)]
        public async Task HTTPPOST_Return200OK_ShouldAllowAdminToStopImpersonation_StopImpersonation_Test()
        {
            const string userName = "amber@test.com";
            string originalUserName = TestHostFixture.AdminLogin().Email;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.Member),
                new Claim("OriginalUserName", originalUserName)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var userManager = _serviceProvider.GetRequiredService<IUserService>();
            var user = await userManager.GetUser(userName);

            var accessToken = jwtAuthManager.GenerateToken(user, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            var response = await _httpClient.PostAsync(baseUrl + "/stop-impersonation", null);
            var responseContent = await TestHostFixture.GetLoginResponseContent(response);
            var result = TestHostFixture.GetLoginResult(responseContent);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual(originalUserName, result.Email);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result.OriginalUserName));
            Assert.IsTrue(result.Roles.Contains(UserRoles.Admin));
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));

            var (principal, jwtSecurityToken) = jwtAuthManager.DecodeJwtToken(result.AccessToken);
            Assert.AreEqual(originalUserName, principal.Identity.Name);
            Assert.IsTrue(principal.HasClaim(claim => claim.Value == UserRoles.Admin));
            Assert.IsTrue(string.IsNullOrWhiteSpace(principal.FindFirst("OriginalUserName")?.Value));
            Assert.IsNotNull(jwtSecurityToken);
        }

        [Test, Order(5)]
        public async Task HTTPPOST_Return400BadRequest_ShouldNotAllowStopWhenNotImpersonating_StopImpersonation_Test()
        {
            string userName = TestHostFixture.AdminLogin().Email;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.Member)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var userManager = _serviceProvider.GetRequiredService<IUserService>();
            var user = await userManager.GetUser(userName);

            var accessToken = jwtAuthManager.GenerateToken(user, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            var request = new ImpersonationRequest { UserName = "amber@test.com" };
            var response = await _httpClient.PostAsync(baseUrl + "/stop-impersonation",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Test, Order(6)]
        public async Task HTTPPOST_Return401Unauthorized_ShouldForbidNonAdminToImpersonate_Impersonate_Test()
        {
            string userName = TestHostFixture.MemberLogin().Email;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.Member)
            };
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var userManager = _serviceProvider.GetRequiredService<IUserService>();
            var user = await userManager.GetUser(userName);

            var accessToken = jwtAuthManager.GenerateToken(user, claims, DateTime.Now.AddMinutes(-1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            var request = new ImpersonationRequest { UserName = "amber@test.com" };
            var response = await _httpClient.PostAsync(baseUrl + "/impersonation",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Test, Order(7)]
        public async Task HTTPPOST_Return401Unauthorized_ShouldForbidNonUserToImpersonate_Impersonate_Test()
        {
            var request = new ImpersonationRequest { UserName = "amber@test.com" };
            var response = await _httpClient.PostAsync(baseUrl + "/impersonation",
                new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, MediaTypeNames.Application.Json));

            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
