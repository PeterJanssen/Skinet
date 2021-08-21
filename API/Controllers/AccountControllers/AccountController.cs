using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Core.Services.Interfaces.Identity;
using Application.Core.Services.Interfaces.Identity.JWT;
using Application.Dtos.AccountDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AccountControllers
{
    [Authorize]
    [Produces("application/json")]
    public class AccountController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AccountController(IUserService userService, IJwtAuthManager jwtAuthManager)
        {
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
        }

        /// <summary>
        /// Gets the current logged in user
        /// </summary>
        /// <response code="200">Returns the current logged in user</response>
        /// <response code="401">If the user is not logged in</response>      
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResult>> GetCurrentUser()
        {
            var user = await _userService.GetUser(User.Identity.Name);
            var roles = await _userService.GetUserRoles(user);

            var userRoles = await _userService.GetUserRoles(user);

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.GivenName, user.DisplayName)
            };

            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtResult = _jwtAuthManager.GenerateTokens(user.Email, claims, DateTime.Now);

            return Ok(CreateLoginResult(
                    user.Email,
                    user.DisplayName,
                    userRoles.ToList(),
                    User.FindFirst("OriginalUserName")?.Value,
                    jwtResult.AccessToken,
                    jwtResult.RefreshToken.TokenString
                    ));
        }

        private static LoginResult CreateLoginResult(
            string email,
            string displayName,
            List<string> roles,
            string originalUsername,
            string accessToken,
            string refreshToken
            )
        {
            return new LoginResult
            {
                Email = email,
                Roles = roles,
                DisplayName = displayName,
                OriginalUserName = originalUsername,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}