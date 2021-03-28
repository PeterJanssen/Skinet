using System.Collections.Generic;
using System.Threading.Tasks;
using API.Helpers.SharedHelpers;
using AutoMapper;
using Core.Entities.ProductEntities;
using Core.Interfaces.Services.ProductServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.ProductsControllers
{
    [Produces("application/json")]
    [Route("api/products/brands")]
    public class ProductBrandsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IProductBrandsService _productBrandsService;
        public ProductBrandsController(IProductBrandsService productBrandsService, IMapper mapper)
        {
            _productBrandsService = productBrandsService;
            _mapper = mapper;
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