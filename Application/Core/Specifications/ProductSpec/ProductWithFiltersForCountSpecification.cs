using Domain.Models.ProductModels;
using Persistence.Data.Repository;

namespace Application.Core.Specifications.ProductSpec
{
    public class ProductWithFiltersForCountSpecification : Specification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecParams specParams) : base(product =>
                (string.IsNullOrEmpty(specParams.Search) || product.Name.ToLower().Contains(specParams.Search)) &&
                (!specParams.BrandId.HasValue || product.ProductBrandId == specParams.BrandId) &&
                (!specParams.TypeId.HasValue || product.ProductTypeId == specParams.TypeId)
                )
        {

        }
    }
}