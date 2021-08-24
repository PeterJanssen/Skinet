using System.Threading.Tasks;
using Application.Dtos.BasketDtos;
using Domain.Models.BasketModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence.Data.Repository.Interfaces;

namespace API.Controllers.BasketControllers
{
    [Produces("application/json")]
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        public BasketController(IBasketRepository basketRepository)
        {
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
        public async Task<ActionResult<CustomerBasketDto>> GetBasketById(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            var basketDto = Mapper.Map<CustomerBasket, CustomerBasketDto>(basket);

            return Ok(basketDto ?? new CustomerBasketDto() { Id = id });
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
        public async Task<ActionResult<CustomerBasketDto>> UpdateBasket(CustomerBasketDto basketDto)
        {
            var customerBasket = Mapper.Map<CustomerBasketDto, CustomerBasket>(basketDto);

            var result = await _basketRepository.UpdateBasketAsync(customerBasket);

            var updatedBasket= Mapper.Map<CustomerBasket, CustomerBasketDto>(result);

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
        /// <response code="200">Returns if the basket is deleted</response>
        /// <response code="400">Returns if the basket could not be deleted</response>
        /// <response code="404">Returns if the basket could not be found</response>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteBasketAsync(string id)
        {
            var basket = await _basketRepository.GetBasketAsync(id);

            if (basket == null) return NotFound("No basket found.");

            var result = await _basketRepository.DeleteBasketAsync(id);

            if (!result) return BadRequest("Basket could not be removed.");

            return Ok();
        }
    }
}