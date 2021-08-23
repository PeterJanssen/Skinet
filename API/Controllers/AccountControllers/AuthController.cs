using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Errors;
using API.Extensions;
using Application.Core.Services.Interfaces.Identity;
using Application.Core.Services.Interfaces.Identity.JWT;
using Application.Dtos.AccountDtos;
using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers.AccountControllers
{
    [Produces("application/json")]
    public class AuthController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AuthController(IUserService userService, IJwtAuthManager jwtAuthManager)
        {
            _userService = userService;
            _jwtAuthManager = jwtAuthManager;
        }

        /// <summary>
        /// Posts the users's credentials and logs the user in
        /// </summary>
        /// <remarks>
        /// Sample request Member:
        ///
        ///     {
        ///         "email": "bob@test.com",
        ///         "password": "Pa$$w0rd"
        ///     }
        ///
        /// Sample request Admin:
        ///
        ///     {
        ///         "email": "admin@test.com",
        ///         "password": "Pa$$w0rd"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the current logged in user</response>
        /// <response code="401">Returns if the User does not exist or has provided wrong credentials</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResult>> Login(LoginRequest request)
        {
            var user = await _userService.GetUser(request.Email);

            if (user == null) return Unauthorized;

            var result = await _userService.SignUserIn(user, request.Password);

            if (!result.Succeeded) return Unauthorized;

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
        /// Posts and logs the user out by removing their tokens
        /// </summary>
        /// <response code="200">Returns Status200OK</response>
        /// <response code="401">Returns if no user is logged in</response>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public ActionResult Logout()
        {
            var userName = HttpContext.User.GetUsername();

            if (userName == null) return Unauthorized;

            _jwtAuthManager.RemoveRefreshTokenByUserName(userName);

            return Ok();
        }

        /// <summary>
        /// Registers the new users's credentials and logs the user in
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "displayName": "Tom",    
        ///         "email": "tom@test.com",
        ///         "password": "Pa$$w0rd"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the newly registered user</response>
        /// <response code="400">Returns if the new user's email is already in use or registering has failed</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginResult>> Register(RegisterRequest registerRequest)
        {
            var existingEmail = await _userService.GetUser(registerRequest.Email);

            if (existingEmail != null)
            {
                return new BadRequestObjectResult(
                    new ApiValidationErrorResponse
                    {
                        Errors = new[] { "Email address is in use" }
                    }
                    );
            }

            var existingUsername = await _userService.GetUser(registerRequest.Email);

            if (existingUsername != null)
            {
                return new BadRequestObjectResult(
                new ApiValidationErrorResponse
                {
                    Errors = new[] { "Username is in use" }
                }
                    );
            }

            var user = new AppUser()
            {
                Email = registerRequest.Email,
                Created = DateTime.Now,
                DisplayName = registerRequest.DisplayName,
                UserName = registerRequest.Email
            };

            var result = await _userService.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded) return BadRequest;

            var roleAddResult = await _userService.AddToRoleAsync(user);

            if (!roleAddResult.Succeeded) return BadRequest;

            return Login(new LoginRequest()
            {
                Email = user.Email,
                Password = registerRequest.Password
            }).Result;
        }

        /// <summary>
        /// Takes the old refreshtoken and returns a LoginResult with the new refreshtoken
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "refreshToken": "PUT YOUR REFRESHTOKEN HERE FROM YOUR LOGINRESULT",
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the new refreshtoken in a LoginResult</response>
        /// <response code="401">Returns if the user is not logged in or a SecurityTokenException occurs</response>
        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult<LoginResult>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var userName = HttpContext.User.GetUsername();

                if (string.IsNullOrWhiteSpace(request.RefreshToken)) return Unauthorized;

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthManager.Refresh(request.RefreshToken, accessToken, DateTime.Now);

                var user = await _userService.GetUser(userName);
                var roles = await _userService.GetUserRoles(user);

                return Ok(CreateLoginResult(
                        user.Email,
                        user.DisplayName,
                        roles.ToList(),
                        User.FindFirst("OriginalUserName")?.Value,
                        jwtResult.AccessToken,
                        jwtResult.RefreshToken.TokenString
                        ));
            }
            catch (SecurityTokenException)
            {
                return Unauthorized;
            }
        }

        /// <summary>
        /// Checks if the filled in email exists in database
        /// </summary>
        /// <remarks>
        /// Sample existing email:
        ///   
        ///     bob@test.com
        ///
        /// </remarks>
        /// <response code="200">Returns true if email exists in database false if not</response>
        [HttpGet("emailexists")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userService.GetUser(email) != null;
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