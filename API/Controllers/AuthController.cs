using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using AutoMapper;
using Core.Entities.Identity;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Produces("application/json")]
    public class AuthController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signingManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signingManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            _signingManager = signingManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
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
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var result = await _signingManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized(new ApiResponse(401));
            }

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (CheckEmailExistsAsync(registerDto.Email).Result.Value)
            {
                return new BadRequestObjectResult(
                    new ApiValidationErrorResponse
                    {
                        Errors = new[] { "Email address is in use" }
                    }
                    );
            }

            var user = _mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Email;

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }

            var roleAddResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleAddResult.Succeeded)
            {
                return BadRequest("Failed to add to role");
            }

            return new UserDto
            {
                Email = user.Email,
                Token = await _tokenService.CreateToken(user),
                DisplayName = user.DisplayName
            };
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> CheckEmailExistsAsync([FromQuery] string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }
    
    }
}