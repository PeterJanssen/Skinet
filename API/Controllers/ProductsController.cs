using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Produces("application/json")]
    public class ProductsController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;
        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all products (with query) and caches them
        /// </summary>
        /// <remarks>
        /// Sample query params:
        ///     
        ///     On Page index:
        ///         1
        ///     Size of page:
        ///         4
        ///     By Brand:
        ///         3
        ///     By Type:
        ///         3
        ///     Sorting:
        ///         priceAsc
        ///         priceDesc
        ///     Search:
        ///         red
        ///         blue
        ///</remarks>
        /// <response code="200">Returns all products with the provided paramets</response>
        [Cached(600)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var products = await _productService.GetProductsAsync(specParams);

            var totalItems = await _productService.CountProductsAsync(specParams);

            var productDto = _mapper.Map<
            IReadOnlyList<Product>,
            IReadOnlyList<ProductToReturnDto>>
            (products);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex, specParams.PageSize, totalItems, productDto));
        }

        /// <summary>
        /// Gets product with provided id
        /// </summary>
        /// <remarks>
        /// Sample id:
        ///
        ///     1
        ///</remarks>
        /// <response code="200">Returns the product with the provided id</response>
        /// <response code="404">Returns if the product is not found</response>
        [Cached(600)]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(new ApiResponse(404));
            }

            var productDto = _mapper.Map<Product, ProductToReturnDto>(product);

            return Ok(productDto);
        }

        /// <summary>
        /// Gets all brands
        /// </summary>
        /// <response code="200">Returns all brands</response>
        [Cached(600)]
        [HttpGet("brands")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var brands = await _productService.GetProductBrandsAsync();

            return Ok(brands);
        }

        /// <summary>
        /// Gets all types
        /// </summary>
        /// <response code="200">Returns all types</response>
        [Cached(600)]
        [HttpGet("types")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            var types = await _productService.GetProductTypesAsync();

            return Ok(types);
        }
    }
}