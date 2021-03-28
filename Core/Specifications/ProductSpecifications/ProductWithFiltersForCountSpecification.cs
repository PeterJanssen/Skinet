using Core.Entities.ProductEntities;

namespace Core.Specifications.ProductSpecifications
{
    public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
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