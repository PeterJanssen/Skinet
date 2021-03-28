using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos.AccountDtos;
using API.Dtos.OrderDtos;
using API.Errors;
using API.Extensions;
using API.Helpers.SharedHelpers;
using AutoMapper;
using Core.Entities.OrderEntities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services.OrderServices;
using Core.Specifications.OrderSpecifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.OrdersControllers
{
    [Authorize]
    [Produces("application/json")]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        public OrdersController(IOrderService orderService, IMapper mapper, IDeliveryMethodService deliveryMethodService, IBasketRepository basketRepository)
        {
            _mapper = mapper;
            _orderService = orderService;
            _deliveryMethodService = deliveryMethodService;
            _basketRepository = basketRepository;
        }


        /// <summary>
        /// Posts a new order
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "basketId": "basket1",
        ///         "deliveryMethodId": 1,
        ///         "shipToAddress": 
        ///         {
        ///             "firstName": "Bob",
        ///             "lastName": "Bobbity",
        ///             "street": "10 The Street",
        ///             "city": "New York",
        ///             "state": "NY",
        ///             "country": "USA",
        ///             "zipcode": "90250"
        ///         }
        ///     }
        /// </remarks>
        /// <response code="200">Returns a newly created order</response>
        /// <response code="400">Returns if the order could not be created</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var address = _mapper.Map<AddressDto, OrderAddress>(orderDto.ShipToAddress);

            var deliveryMethod = await _deliveryMethodService.GetDeliveryMethodByIdAsync(orderDto.DeliveryMethodId);

            var basket = await _basketRepository.GetBasketAsync(orderDto.BasketId);

            if (email == null || address == null || deliveryMethod == null || basket == null)
            {
                return BadRequest(new ApiResponse(400, "Problem creating order"));
            }

            var order = await _orderService.CreateOrderAsync(email, deliveryMethod, basket, address);

            if (order == null)
            {
                return BadRequest(new ApiResponse(400, "Problem creating order"));
            }

            return Ok(order);
        }

        /// <summary>
        /// Gets all orders for the current user
        /// </summary>
        /// <response code="200">Returns all orders for the current user</response>
        /// <response code="401">Returns if user is not logged in</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Pagination<OrderDto>>> GetOrdersForUser([FromQuery] OrderSpecParams specParams)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var orders = await _orderService.GetOrdersForUserAsync(specParams, email);

            var totalItems = await _orderService.CountProductsAsync(specParams, email);

            var orderToReturnDto = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);

            return Ok(new Pagination<OrderToReturnDto>(specParams.PageIndex, specParams.PageSize, totalItems, orderToReturnDto));
        }

        /// <summary>
        /// Gets order for the current user provided by Id
        /// </summary>
        /// <remarks>
        /// Sample id:
        ///
        ///     1
        ///</remarks>
        /// <response code="200">Returns the order with the provided id for the current user</response>
        /// <response code="401">Returns if user is not logged in</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var order = await _orderService.GetOrderByIdAsync(id, email);

            if (order == null)
            {
                return NotFound(new ApiResponse(404));
            }

            return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
        }
    }
}