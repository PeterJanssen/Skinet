using System.Threading.Tasks;
using API.Dtos;
using API.Extensions;
using AutoMapper;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/account/[controller]")]
    public class AddressController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public AddressController(
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        /// <summary>
        /// Gets the current logged in user's address
        /// </summary>
        /// <response code="200">Returns the current logged in user's address</response>
        /// <response code="401">Returns if the User is not logged in</response>
        [Authorize]
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AddressDto>> getUserAddress()
        {
            var user = await _userManager.FindUserByClaimsPrincipleWithAddressAsync(User);

            return _mapper.Map<Address, AddressDto>(user.Address);
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
        /// <response code="401">Returns if the User is not logged in</response>
        [Authorize]
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto)
        {
            var user = await _userManager.FindUserByClaimsPrincipleWithAddressAsync(User);
            user.Address = _mapper.Map<AddressDto, Address>(addressDto);

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(_mapper.Map<Address, AddressDto>(user.Address));
            }

            return BadRequest("Updating user failed");
        }

    }
}