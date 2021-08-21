using System.Collections.Generic;
using System.Threading.Tasks;
using API.Caching;
using Application.Core.Services.Interfaces.ProductServices;
using Domain.Models.ProductModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.ProductsControllers
{
    [Produces("application/json")]
    [Route("api/products/brands")]
    public class ProductBrandsController : BaseApiController
    {
        private readonly IProductBrandsService _productBrandsService;
        public ProductBrandsController(IProductBrandsService productBrandsService)
        {
            _productBrandsService = productBrandsService;
        }

        /// <summary>
        /// Gets all brands
        /// </summary>
        /// <response code="200">Returns all brands</response>
        [Cached(600)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var brands = await _productBrandsService.GetProductBrandsAsync();

            return Ok(brands);
        }
    }
}