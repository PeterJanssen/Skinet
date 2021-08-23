using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Extensions;
using Application.Core.Services.Implementations.Identity;
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
        /// <response code="400">If the user is not found</response>  
        /// <response code="401">If the user is not logged in</response>      
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResult>> GetCurrentUser()
        {
            var user = await _userService.GetUser(User.Identity.Name);

            if (user == null) return BadRequest;

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

        /// <summary>
        /// Impersonates the given user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///   
        ///     {
        ///         "UserName": "bob@test.com"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the impersonated user</response>
        /// <response code="400">If the user or impersonated user is not found</response>  
        /// <response code="401">If the user is not an admin or not logged in</response> 
        [HttpPost("impersonation")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> Impersonate([FromBody] ImpersonationRequest request)
        {
            var userName = HttpContext.User.GetUsername();

            var user = await _userService.GetUser(User.Identity?.Name);

            if (user == null) return BadRequest;

            var impersonatedUser = await _userService.GetUser(request.UserName);
            var impersonatedRoles = await _userService.GetUserRoles(impersonatedUser);

            if (impersonatedRoles.Contains(UserRoles.Admin)) return BadRequest;

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Email, impersonatedUser.Email),
                new Claim(ClaimTypes.Name, impersonatedUser.UserName),
                new Claim(ClaimTypes.GivenName, impersonatedUser.DisplayName),
                new Claim("OriginalUserName", userName ?? string.Empty)
            };

            claims.AddRange(impersonatedRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtResult = _jwtAuthManager.GenerateTokens(request.UserName, claims, DateTime.Now);

            return Ok(CreateLoginResult(
                    request.UserName,
                    impersonatedUser.DisplayName,
                    impersonatedRoles.ToList(),
                    userName,
                    jwtResult.AccessToken,
                    jwtResult.RefreshToken.TokenString
                    ));
        }

        /// <summary>
        /// Stops the impersonation of the given user
        /// </summary>
        /// <response code="200">Returns the original user</response>
        /// <response code="400">If the user or original user is not found</response>  
        [HttpPost("stop-impersonation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> StopImpersonation()
        {
            var user = await _userService.GetUser(User.Identity.Name);

            var originalUser = await _userService.GetUser(User.FindFirst("OriginalUserName")?.Value);

            if (originalUser == null) return BadRequest;

            var roles = await _userService.GetUserRoles(originalUser);

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Email, originalUser.Email),
                new Claim(ClaimTypes.Name, originalUser.UserName),
                new Claim(ClaimTypes.GivenName, originalUser.DisplayName),
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwtResult = _jwtAuthManager.GenerateTokens(originalUser.UserName, claims, DateTime.Now);

            return Ok(CreateLoginResult(
                    originalUser.UserName,
                    originalUser.DisplayName,
                    roles.ToList(),
                    null,
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