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
    public class AccountController : BaseAccountController
    {
        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AccountController(IUserService userService, IJwtAuthManager jwtAuthManager) : base(userService, jwtAuthManager)
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

            if (user == null) return BadRequest("Current user not found.");

            var roles = await _userService.GetUserRoles(user);

            var userRoles = await _userService.GetUserRoles(user);

            List<Claim> claims = GetClaimsForUser(user, userRoles);

            var accessToken = _jwtAuthManager.GenerateToken(user, claims, DateTime.Now);

            await SetRefreshToken(user, accessToken);

            return Ok(CreateLoginResult(
                    user.Email,
                    user.DisplayName,
                    userRoles.ToList(),
                    User.FindFirst("OriginalUserName")?.Value,
                    accessToken
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
        /// <response code="401">If the user is not logged in</response> 
        /// <response code="403">If the user is not an admin</response> 
        [HttpPost("impersonation")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Impersonate([FromBody] ImpersonationRequest request)
        {
            var userName = HttpContext.User.GetUsername();

            var user = await _userService.GetUser(User.Identity?.Name);

            if (user == null) return BadRequest("Current user not found.");

            var impersonatedUser = await _userService.GetUser(request.UserName);
            var impersonatedRoles = await _userService.GetUserRoles(impersonatedUser);

            if (impersonatedRoles.Contains(UserRoles.Admin)) return BadRequest("Can't impersonate an admin.");

            List<Claim> claims = GetClaimsForUser(impersonatedUser, impersonatedRoles);

            claims.Add(new Claim("OriginalUserName", userName ?? string.Empty));

            var accessToken = _jwtAuthManager.GenerateToken(impersonatedUser, claims, DateTime.Now);

            await SetRefreshToken(impersonatedUser, accessToken);

            return Ok(CreateLoginResult(
                    request.UserName,
                    impersonatedUser.DisplayName,
                    impersonatedRoles.ToList(),
                    userName,
                    accessToken
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

            if (originalUser == null) return BadRequest("Original user is not found.");

            var roles = await _userService.GetUserRoles(originalUser);

            List<Claim> claims = GetClaimsForUser(originalUser, roles);

            var accessToken = _jwtAuthManager.GenerateToken(originalUser, claims, DateTime.Now);

            await SetRefreshToken(originalUser, accessToken);

            return Ok(CreateLoginResult(
                    originalUser.UserName,
                    originalUser.DisplayName,
                    roles.ToList(),
                    null,
                    accessToken
                    ));
        }
    }
}