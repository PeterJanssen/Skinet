using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Core.Services.Interfaces.OrderServices;
using Application.Dtos.OrderDtos;
using Domain.Models.OrderModels;
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
        public DeliveryMethodsController(IDeliveryMethodService deliveryMethodService)
        {
            _deliveryMethodService = deliveryMethodService;
        }

        /// <summary>
        /// Gets all delivery methods
        /// </summary>
        /// <response code="200">Returns all delivery methods</response>
        /// <response code="401">Returns if user is not logged in</response>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethodDto>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _deliveryMethodService.GetDeliveryMethodsAsync();

            var deliveryMethodDtoList = Mapper.Map<
            IReadOnlyList<DeliveryMethod>,
            IReadOnlyList<DeliveryMethodDto>>
            (deliveryMethods);

            return Ok(deliveryMethodDtoList);
        }
    }
}