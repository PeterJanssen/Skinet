using Application.Core.Services.Implementations.Identity;
using Application.Core.Services.Interfaces.Identity.JWT;
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
using Domain.Models.AccountModels.JWT;
using Application.Core.Services.Interfaces.Identity;

namespace API_tests.Controllers.AccountControllers
{
    class AuthController
    {
        private readonly TestHostFixture _testHostFixture = new();
        private HttpClient _httpClient;
        private IServiceProvider _serviceProvider;
        private const string baseUrl = "api/auth";

        [SetUp()]
        public void SetUp()
        {
            _httpClient = _testHostFixture.Client;
            _serviceProvider = _testHostFixture.ServiceProvider;
        }

        [Test]
        public async Task HTTPPOST_Return401Unauthorized_ShouldForbidInvalidCredentials_Login_Test()
        {
            var loginResponse = await _testHostFixture.GetLoginResponse(TestHostFixture.InvalidLogin());
            Assert.AreEqual(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
        }

        [Test]
        public async Task HTTPPOST_Return200OK_ShouldReturnLoginResult_Login_Test()
        {
            var credentials = TestHostFixture.AdminLogin();

            var loginResponse = await _testHostFixture.GetLoginResponse(credentials);
            Assert.AreEqual(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginResponseContent = await TestHostFixture.GetLoginResponseContent(loginResponse);
            var loginResult = TestHostFixture.GetLoginResult(loginResponseContent);

            Assert.AreEqual(credentials.Email, loginResult.Email);
            Assert.IsNull(loginResult.OriginalUserName);
            Assert.IsTrue(loginResult.Roles.Contains(UserRoles.Admin));
            Assert.IsFalse(string.IsNullOrWhiteSpace(loginResult.AccessToken));

            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var (principal, jwtSecurityToken) = jwtAuthManager.DecodeJwtToken(loginResult.AccessToken);
            Assert.AreEqual(credentials.Email, principal.Identity.Name);
            Assert.IsTrue(principal.HasClaim(claim => claim.Value == UserRoles.Admin));
            Assert.IsNotNull(jwtSecurityToken);
        }

        [Test]
        public async Task HTTPPOST_Return200OK_ShouldBeAbleToLogout_Logout_Test()
        {
            var credentials = TestHostFixture.AdminLogin();

            var loginResponse = await _testHostFixture.GetLoginResponse(credentials);
            var loginResponseContent = await TestHostFixture.GetLoginResponseContent(loginResponse);
            var loginResult = TestHostFixture.GetLoginResult(loginResponseContent);

            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, loginResult.AccessToken);
            var logoutResponse = await _httpClient.PostAsync(baseUrl + "/logout", null);
            Assert.AreEqual(HttpStatusCode.OK, logoutResponse.StatusCode);
        }

        [Test]
        public async Task HTTPPOST_Return200OK_ShouldCorrectlyRefreshToken_RefreshToken_Test()
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

            var response = await _httpClient.PostAsync(baseUrl + "/refresh-token", null);
            var responseContent = await TestHostFixture.GetLoginResponseContent(response);
            var result = TestHostFixture.GetLoginResult(responseContent);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }


        [Test]
        public async Task HTTPPOST_Return401Unauthorized_ShouldNotAllowWhenAccessTokenIsExpired_RefreshToken_Test()
        {
            string userName = TestHostFixture.AdminLogin().Email;
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,userName),
                new Claim(ClaimTypes.Role, UserRoles.Admin)
            };
            var jwtTokenConfig = _serviceProvider.GetRequiredService<JwtTokenConfig>();
            var jwtAuthManager = _serviceProvider.GetRequiredService<IJwtAuthManager>();
            var userManager = _serviceProvider.GetRequiredService<IUserService>();
            var user = await userManager.GetUser(userName);

            var accessToken = jwtAuthManager.GenerateToken(user, claims, DateTime.Now.AddMinutes(-jwtTokenConfig.RefreshTokenExpiration - 1));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken); // Expired JWT token

            var response = await _httpClient.PostAsync(baseUrl + "/refresh-token", null);
            var responseContent = await TestHostFixture.GetLoginResponseContent(response);
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.IsTrue(responseContent.Contains("Unauthorized action"));
        }
    }
}
