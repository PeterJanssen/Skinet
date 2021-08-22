using System.Threading.Tasks;
using Application.Core.Services.Interfaces.Identity;
using Application.Dtos.AccountDtos;
using Domain.Models.AccountModels.AppUserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.AccountControllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/account/[controller]")]
    public class AddressController : BaseApiController
    {
        private readonly IUserService _userService;
        public AddressController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Gets the current logged in user's address
        /// </summary>
        /// <response code="200">Returns the current logged in user's address</response>
        /// <response code="204">Returns if the current logged in user does not have an address</response>
        /// <response code="401">Returns if the User is not logged in</response>
        [Authorize]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userService.FindUserByClaimsPrincipleWithAddressAsync(User);

            return Ok(Mapper.Map<Address, AddressDto>(user.Address));
        }

        /// <summary>
        /// Updates the current logged in user's address
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///   
        ///     {
        ///         "firstName": "Bob",
        ///         "lastName": "Bobbity",
        ///         "street": "10 The Updated Street",
        ///         "city": "New York",
        ///         "state": "NY",
        ///         "zipCode": "90250",
        ///         "country": "USA"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the current logged in user's updated address</response>
        /// <response code="400">Returns if the User could not be updated</response>
        /// <response code="401">Returns if the User is not logged in</response>
        [Authorize]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        {
            var user = await _userService.FindUserByClaimsPrincipleWithAddressAsync(User);
            user.Address = Mapper.Map<AddressDto, Address>(addressDto);

            var result = await _userService.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(Mapper.Map<Address, AddressDto>(user.Address));
            }

            return BadRequest;
        }

    }
}