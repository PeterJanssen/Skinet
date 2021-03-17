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
        private readonly IGenericRepository<Product> _productsRepo;
        private readonly IGenericRepository<ProductBrand> _productBrandsRepo;
        private readonly IGenericRepository<ProductType> _productTypesRepo;
        private readonly IMapper _mapper;
        public ProductsController(IGenericRepository<Product> productsRepo,
            IGenericRepository<ProductBrand> productBrandsRepo,
            IGenericRepository<ProductType> productTypesRepo,
            IMapper mapper)
        {
            _mapper = mapper;
            _productTypesRepo = productTypesRepo;
            _productBrandsRepo = productBrandsRepo;
            _productsRepo = productsRepo;

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
            var specification = new ProductsWithTypesAndBrandsSepcification(specParams);
            var countSpec = new ProductWithFiltersForCountSpecification(specParams);

            var totalItems = await _productsRepo.CountAsync(countSpec);

            var products = await _productsRepo.ListAsync(specification);

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
            var specification = new ProductsWithTypesAndBrandsSepcification(id);

            var product = await _productsRepo.GetEntityWithSpec(specification);

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
            var brands = await _productBrandsRepo.ListAllAsync();

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
            var types = await _productTypesRepo.ListAllAsync();

            return Ok(types);
        }
    }
}