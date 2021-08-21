using System.Collections.Generic;
using System.Threading.Tasks;
using API.Extensions;
using Application.Core.Paging;
using Application.Core.Services.Interfaces.OrderServices;
using Application.Core.Specifications.OrderSpec;
using Application.Dtos.AccountDtos;
using Application.Dtos.OrderDtos;
using Domain.Models.OrderModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Data.Repository.Interfaces;

namespace API.Controllers.OrdersControllers
{
    [Authorize]
    [Produces("application/json")]
    public class OrdersController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IDeliveryMethodService _deliveryMethodService;
        private readonly IBasketRepository _basketRepository;
        public OrdersController(
            IOrderService orderService,
            IDeliveryMethodService deliveryMethodService,
            IBasketRepository basketRepository
            )
        {
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var address = Mapper.Map<AddressDto, OrderAddress>(orderDto.ShipToAddress);

            var deliveryMethod = await _deliveryMethodService.GetDeliveryMethodByIdAsync(orderDto.DeliveryMethodId);

            var basket = await _basketRepository.GetBasketAsync(orderDto.BasketId);

            if (email == null || address == null || deliveryMethod == null || basket == null) return BadRequest;

            var order = await _orderService.CreateOrderAsync(email, deliveryMethod, basket, address);

            if (order == null) return BadRequest;

            return Ok(order);
        }

        /// <summary>
        /// Gets all orders for the current user
        /// </summary>
        /// <response code="200">Returns all orders for the current user</response>
        /// <response code="401">Returns if user is not logged in</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Pagination<OrderDto>>> GetOrdersForUser([FromQuery] OrderSpecParams specParams)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var orders = await _orderService.GetOrdersForUserAsync(specParams, email);

            var totalItems = await _orderService.CountProductsAsync(specParams, email);

            var orderToReturnDto = Mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders);

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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var email = HttpContext.User.RetrieveEmailFromPrincipal();

            var order = await _orderService.GetOrderByIdAsync(id, email);

            if (order == null) return NotFound;

            return Ok(Mapper.Map<Order, OrderToReturnDto>(order));
        }
    }
}