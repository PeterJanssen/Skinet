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
        /// <summary>
        /// Posts a new product
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "name": "Test Product",
        ///         "description": "Test product Creation",
        ///         "price": 99.99,
        ///         "pictureUrl": "images/products/placeholder.png",
        ///         "productTypeId": 2,
        ///         "productBrandId": 3
        ///     }
        /// </remarks>
        /// <response code="200">Returns a newly created product</response>
        /// <response code="400">Returns if the product could not be created</response>
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(ProductCreateDto productToCreate)
        {
            var product = _mapper.Map<ProductCreateDto, Product>(productToCreate);
            product.PictureUrl = "images/products/placeholder.png";

            var result = await _productService.CreateProductAsync(product);

            if (result <= 0)
            {
                return BadRequest(new ApiResponse(400, "Product could not be created, please try again."));
            }

            return Ok(product);
        }
        /// <summary>
        /// Updates a product with the provided id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///   
        ///     {
        ///         "name": "Updated Test Product",
        ///         "description": "Updated Test product Creation",
        ///         "price": 999.99,
        ///         "pictureUrl": "images/products/placeholder.png",
        ///         "productTypeId": 3,
        ///         "productBrandId": 4
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the updated product</response>
        /// <response code="400">Returns if the product could not be updated</response>
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, ProductCreateDto productToUpdate)
        {
            var product = await _productService.GetProductByIdAsync(id);

            _mapper.Map(productToUpdate, product);

            var result = await _productService.UpdateProductAsync(product);

            if (result <= 0)
            {
                return BadRequest(new ApiResponse(400, "Product could not be updated, please try again."));
            }

            return Ok(product);
        }
        /// <summary>
        /// Deletes a product with the provided id
        /// </summary>
        /// <remarks>
        /// Sample id:
        ///   
        ///     1
        ///
        /// </remarks>
        /// <response code="200">Returns if the product is deleted</response>
        /// <response code="400">Returns if the product could not be deleted</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            var result = await _productService.DeleteProductAsync(product);

            if (result <= 0)
            {
                return BadRequest(new ApiResponse(400, "Product could not be deleted, please try again."));
            }

            return Ok();
        }
    }
}