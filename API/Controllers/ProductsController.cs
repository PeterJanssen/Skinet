using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
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

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts()
        {
            var specification = new ProductsWithTypesAndBrandsSepcification();

            var products = await _productsRepo.ListAsync(specification);

            var productDto = _mapper.Map<
            IReadOnlyList<Product>,
            IReadOnlyList<ProductToReturnDto>>
            (products);

            return Ok(productDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var specification = new ProductsWithTypesAndBrandsSepcification(id);

            var product = await _productsRepo.GetEntityWithSpec(specification);

            var productDto = _mapper.Map<Product, ProductToReturnDto>(product);

            return Ok(productDto);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var brands = await _productBrandsRepo.ListAllasync();

            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            var types = await _productTypesRepo.ListAllasync();

            return Ok(types);
        }
    }
}