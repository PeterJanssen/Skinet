using System.Collections.Generic;
using System.Threading.Tasks;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Produces("application/json")]
    [Route("api/products/types")]
    public class ProductTypesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IProductTypesService _productTypesService;
        public ProductTypesController(IProductTypesService productTypesService, IMapper mapper)
        {
            _productTypesService = productTypesService;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all types
        /// </summary>
        /// <response code="200">Returns all types</response>
        [Cached(600)]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {
            var types = await _productTypesService.GetProductTypesAsync();

            return Ok(types);
        }
    }
}