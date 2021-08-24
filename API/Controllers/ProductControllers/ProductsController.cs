using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Caching;
using Application.Core.Paging;
using Application.Core.Services.Implementations.Identity;
using Application.Core.Services.Interfaces.ProductServices;
using Application.Core.Specifications.ProductSpec;
using Application.Dtos.ProductDtos;
using Domain.Models.ProductModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.ProductsControllers
{
    [Produces("application/json")]
    public class ProductsController : BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IPhotoService _photoService;
        public ProductsController(IProductService productService, IPhotoService photoService)
        {
            _productService = productService;
            _photoService = photoService;
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
        /// <response code="200">Returns all products with the provided parameters</response>
        //[Cached(600)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            var products = await _productService.GetProductsAsync(specParams);

            var totalItems = await _productService.CountProductsAsync(specParams);

            var productDto = Mapper.Map<
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null) return NotFound("Product not found.");

            var productDto = Mapper.Map<Product, ProductToReturnDto>(product);

            return Ok(productDto);
        }

        /// <summary>
        /// Updates a product and adds a photo with the provided id
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
        /// <response code="400">Returns if the product could not be updated or the photo could not be written to disk</response>
        /// <response code="403">Returns if the user is not logged in</response>
        /// <response code="403">Returns if the current user is not an admin</response>
        /// <response code="404">Returns if the product could not be found</response>
        [HttpPut("{id}/photo")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> AddProductPhoto(int id, [FromForm] ProductPhotoDto photoDto)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null) return NotFound("Product not found.");

            if (photoDto.Photo.Length > 0)
            {
                var photo = await _photoService.SaveToDiskAsync(photoDto.Photo);

                if (photo != null)
                {
                    product.AddPhoto(photo.PictureUrl, photo.FileName);

                    var result = await _productService.UpdateProductAsync(product);

                    if (result <= 0)
                    {
                        return BadRequest("Photo could not be saved.");
                    }
                }
                else
                {
                    return BadRequest("No foto provided in request.");
                }
            }

            var productDto = Mapper.Map<Product, ProductToReturnDto>(product);

            return productDto;
        }

        /// <summary>
        /// Adds a review to a product with the provided id
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///   
        ///     {
        ///         "rating": 3,
        ///         "reviewText": "Good Product"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns the updated product</response>
        /// <response code="400">Returns if the product could not be updated</response>
        /// <response code="401">Returns if the user is not logged in</response>
        /// <response code="404">Returns if the product could not be found</response>
        [HttpPut("{id}/rating")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> AddProductReview(int id, ProductReviewDto ProductReview)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null) return NotFound("Product not found.");

            product.AddReview(ProductReview.Rating, ProductReview.ReviewText);

            var result = await _productService.UpdateProductAsync(product);

            if (result <= 0) return BadRequest("Product could not be updated.");

            var productDto = Mapper.Map<Product, ProductToReturnDto>(product);

            return productDto;
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
        /// <response code="401">Returns if the user is not logged in</response>
        /// <response code="403">Returns if the current user is not an admin</response>
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ProductToReturnDto>> CreateProduct(ProductCreateDto productToCreate)
        {
            var product = Mapper.Map<ProductCreateDto, Product>(productToCreate);

            var result = await _productService.CreateProductAsync(product);

            if (result <= 0) return BadRequest("Product could not be created.");

            var createdProduct = Mapper.Map<Product, ProductToReturnDto>(product);

            return Ok(createdProduct);
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
        /// <response code="401">Returns if the user is not logged in</response>
        /// <response code="403">Returns if the current user is not an admin</response>
        /// <response code="404">Returns if the product could not be found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> UpdateProduct(int id, ProductCreateDto productToUpdate)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null) return NotFound("Product not found.");

            Mapper.Map(productToUpdate, product);

            var result = await _productService.UpdateProductAsync(product);

            if (result <= 0) return BadRequest("Product could not be updated.");

            var updatedProduct = Mapper.Map<Product, ProductToReturnDto>(product);

            return Ok(updatedProduct);
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
        /// <response code="401">Returns if the user is not logged in</response>
        /// <response code="403">Returns if the current user is not an admin</response>
        /// <response code="404">Returns if the product could not be found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null) return NotFound("Product not found.");

            foreach (var photo in product.Photos)
            {
                if (photo.Id > 18)
                {
                    _photoService.DeleteFromDisk(photo);
                }
            }

            var result = await _productService.DeleteProductAsync(product);

            if (result <= 0) return BadRequest("Product could not be deleted.");

            return Ok();
        }

        /// <summary>
        /// Deletes a product photo with the provided id's
        /// </summary>
        /// <remarks>
        /// Sample id:
        ///   
        ///     1
        ///
        /// </remarks>
        /// <response code="200">Returns if the product photo is deleted</response>
        /// <response code="400">Returns if the product photo could not be deleted</response>
        /// <response code="401">Returns if the user is not logged in</response>
        /// <response code="403">Returns if the current user is not an admin</response>
        /// <response code="404">Returns if the product or photo could not be found</response>
        [HttpDelete("{id}/photo/{photoId}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteProductPhoto(int id, int photoId)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null || product.Photos.All(photo => photo.Id != photoId)) return NotFound("Product or photo not found.");

            var photo = product.Photos.SingleOrDefault(photo => photo.Id == photoId);

            if (photo != null)
            {
                if (photo.IsMain)
                {
                    return BadRequest("Your main photo can not be deleted.");
                }

                _photoService.DeleteFromDisk(photo);
            }
            else
            {
                return NotFound("Photo not found.");
            }

            product.RemovePhoto(photoId);

            var result = await _productService.UpdateProductAsync(product);

            if (result <= 0) return BadRequest("Photo could not be removed from product.");

            return Ok();
        }

        /// <summary>
        /// Sets a photo as the main photo for a product with the provided id
        /// </summary>
        /// <remarks>
        /// Sample id:
        ///   
        ///     1
        ///
        /// </remarks>
        /// <response code="200">Returns if the product is updated</response>
        /// <response code="400">Returns if the product photo could not be set as main</response>
        /// <response code="401">Returns if the user is not logged in</response>
        /// <response code="403">Returns if the current user is not an admin</response>
        /// <response code="404">Returns if the product or photo could not be found</response>
        [HttpPost("{id}/photo/{photoId}")]
        [Authorize(Roles = UserRoles.Admin)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDto>> SetMainPhoto(int id, int photoId)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null || product.Photos.All(photo => photo.Id != photoId)) return NotFound("Product or photo not found");

            product.SetMainPhoto(photoId);

            var result = await _productService.UpdateProductAsync(product);

            if (result <= 0) return BadRequest("Photo could not be set to main photo.");

            var updatedProduct = Mapper.Map<Product, ProductToReturnDto>(product);

            return Ok(updatedProduct);
        }
    }
}