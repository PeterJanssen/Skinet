using System.Collections.Generic;
using System.Threading.Tasks;
using API.Caching;
using Application.Core.Services.Interfaces.ProductServices;
using Application.Dtos.ProductDtos;
using Domain.Models.ProductModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.ProductsControllers
{
    [Produces("application/json")]
    [Route("api/products/types")]
    public class ProductTypesController : BaseApiController
    {
        private readonly IProductTypesService _productTypesService;
        public ProductTypesController(IProductTypesService productTypesService)
        {
            _productTypesService = productTypesService;
        }

        /// <summary>
        /// Gets all types
        /// </summary>
        /// <response code="200">Returns all types</response>
        [Cached(600)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductTypeDto>>> GetProductTypes()
        {
            var types = await _productTypesService.GetProductTypesAsync();

            var productTypeDtoList = Mapper.Map<
            IReadOnlyList<ProductType>,
            IReadOnlyList<ProductTypeDto>>
            (types);

            return Ok(productTypeDtoList);
        }
    }
}