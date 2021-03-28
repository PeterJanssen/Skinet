using System.Collections.Generic;
using System.Threading.Tasks;
using API.Errors;
using AutoMapper;
using Core.Entities.OrderEntities;
using Core.Interfaces.Services.OrderServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.OrdersControllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/orders/[controller]")]
    public class DeliveryMethodsController : BaseApiController
    {
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IMapper _mapper;
        public DeliveryMethodsController(IDeliveryMethodService deliveryMethodService, IMapper mapper)
        {
            _mapper = mapper;
            _deliveryMethodService = deliveryMethodService;
        }

        /// <summary>
        /// Gets all delivery methods
        /// </summary>
        /// <response code="200">Returns all delivery methods</response>
        /// <response code="401">Returns if user is not logged in</response>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _deliveryMethodService.GetDeliveryMethodsAsync();

            return Ok(deliveryMethods);
        }
    }
}