using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Produces("application/json")]
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _mapper = mapper;
            _basketRepository = basketRepository;
        }

        /// <summary>
        /// Gets the basket with the provided id
        /// </summary>
        /// <remarks>
        /// Sample basket (if it exists):
        ///     
        ///     basket1
        /// </remarks>
        /// <response code="200">Returns a basket with the provided id if the basket id does not exist returns a new basket</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        /// <summary>
        /// Updates the basket of the current user 
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "id": "basket1",
        ///         "items": [
        ///             {
        ///             "id": 4,
        ///             "productName": ".NET Black &amp; White Mug",
        ///             "price": 8.22,
        ///             "quantity": 32,
        ///             "pictureUrl": "https://localhost:5001/images/products/2.png",
        ///             "brand": ".NET",
        ///             "type": "USB Memory Stick"
        ///             }
        ///                 ]
        ///     }
        /// </remarks>
        /// <response code="200">Updates a basket with the provided id if the basket id does not exist returns a new basket</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basketDto)
        {
            var customerBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basketDto);

            var updatedBasket = await _basketRepository.UpdateBasketAsync(customerBasket);

            return Ok(updatedBasket);
        }


        /// <summary>
        /// Deletes the basket with the provided id
        /// </summary>
        /// <remarks>
        /// Sample id:
        ///
        ///     basket1
        /// </remarks>
        /// <response code="200">Deletes a basket with the provided id</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteBasketAsync(string id)
        {
            await _basketRepository.DeleteBasketAsync(id);
        }
    }
}