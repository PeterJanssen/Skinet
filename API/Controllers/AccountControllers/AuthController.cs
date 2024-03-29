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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers.AccountControllers
{
    [Produces("application/json")]
    public class AuthController : BaseAccountController
    {
        private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AuthController(IUserService userService, IJwtAuthManager jwtAuthManager) : base(userService, jwtAuthManager)
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
        /// <response code="401">Returns if the user does not exist or has provided wrong credentials</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResult>> Login(LoginRequest request)
        {
            var user = await _userService.GetUser(request.Email);

            if (user == null) return Unauthorized("No user found for the provided email.");

            var result = await _userService.SignUserIn(user, request.Password);

            if (!result.Succeeded) return Unauthorized("Invalid login credentials");

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
        /// Posts the users's external credentials and logs the user in
        /// </summary>
        /// <response code="200">Returns the current logged in user</response>
        /// <response code="401">Returns if the user does not exist or has provided wrong credentials</response>
        [HttpPost("login/google")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResult>> GoogleLogin(GoogleLoginRequest googleLoginRequest)
        {
            var payload = await _jwtAuthManager.VerifyGoogleToken(googleLoginRequest);

            if (payload == null) return Unauthorized("Invalid External Authentication.");

            var info = new UserLoginInfo(googleLoginRequest.Provider, payload.Subject, googleLoginRequest.Provider);

            var user = await _userService.GetUserByUserLoginInfo(info);

            if (user == null)
            {
                user = await _userService.GetUser(payload.Email);
                if (user == null)
                {
                    user = new AppUser { Email = payload.Email, UserName = payload.Email, DisplayName = payload.Email.Split('@').First() };
                    await _userService.CreateAsync(user);

                    //TODO prepare and send an email for the email confirmation

                    await _userService.AddToRoleAsync(user);
                    await _userService.AddExternalLogin(user, info);
                }
                else
                {
                    await _userService.AddExternalLogin(user, info);
                }
            }

            if (user == null) return Unauthorized("Invalid External Authentication.");

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
        /// Posts and logs the user out by removing their tokens
        /// </summary>
        /// <response code="200">Returns if the user could be logged out</response>
        [HttpPost("logout")]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            var userName = HttpContext.User.GetUsername();

            if (userName == null) return Ok();

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

            if (!result.Succeeded) return BadRequest("User could not be created.");

            var roleAddResult = await _userService.AddToRoleAsync(user);

            if (!roleAddResult.Succeeded) return BadRequest("User could not be added to role.");

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
        /// <response code="400">Returns if the refreshtoken could not be created</response>
        /// <response code="401">Returns if the refreshtoken could not be found or made</response>
        [HttpPost("refresh-token")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResult>> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                var userName = HttpContext.User.GetUsername();
                var user = await _userService.GetUser(userName);

                if (user == null) return Unauthorized();

                var oldToken = user.RefreshTokens.SingleOrDefault(ExistingRefreshToken => ExistingRefreshToken.TokenString == refreshToken);

                if (oldToken != null && !oldToken.IsActive) return Unauthorized();

                var roles = await _userService.GetUserRoles(user);

                List<Claim> claims = GetClaimsForUser(user, roles);

                var accessToken = _jwtAuthManager.GenerateToken(user, claims, DateTime.Now);

                return Ok(CreateLoginResult(
                        user.Email,
                        user.DisplayName,
                        roles.ToList(),
                        User.FindFirst("OriginalUserName")?.Value,
                        accessToken
                        ));
            }
            catch (SecurityTokenException)
            {
                return Unauthorized();
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
    }
}