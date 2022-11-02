using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Core.Services.Interfaces.Identity;
using Application.Core.Services.Interfaces.Identity.JWT;
using Application.Dtos.AccountDtos;
using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace API.Controllers.AccountControllers
{
    public class BaseAccountController : BaseApiController
    {

        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;

        public BaseAccountController(IUserService userService, IJwtAuthManager jwtAuthManager)
        {
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
        }
        protected LoginResult CreateLoginResult(
            string email,
            string displayName,
            List<string> roles,
            string originalUsername,
            string accessToken
            )
        {
            return new LoginResult
            {
                Email = email,
                Roles = roles,
                DisplayName = displayName,
                OriginalUserName = originalUsername,
                AccessToken = accessToken
            };
        }

        protected List<Claim> GetClaimsForUser(AppUser user, IList<string> userRoles)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, user.DisplayName)
            };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claims;
        }

        protected async Task SetRefreshToken(AppUser appUser, string accessToken)
        {
            var refreshToken = _jwtAuthManager.Refresh(appUser, accessToken, DateTime.Now);

            appUser.RefreshTokens.Add(refreshToken);

            await _userService.UpdateAsync(appUser);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Secure = true
            };

            Response.Cookies.Append("refreshToken", refreshToken.TokenString, cookieOptions);
        }
    }
}