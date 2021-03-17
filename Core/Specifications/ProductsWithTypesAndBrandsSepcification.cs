using Core.Entities;

namespace Core.Specifications
{
    public class ProductsWithTypesAndBrandsSepcification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSepcification(ProductSpecParams specParams)
         : base(product =>
                (string.IsNullOrEmpty(specParams.Search) || product.Name.ToLower().Contains(specParams.Search)) &&
                (!specParams.BrandId.HasValue || product.ProductBrandId == specParams.BrandId) &&
                (!specParams.TypeId.HasValue || product.ProductTypeId == specParams.TypeId)
                )
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
            AddOrderBy(product => product.Name);
            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(n => n.Name);
                        break;
                }
            }
        }

        public ProductsWithTypesAndBrandsSepcification(int id) : base(product => product.Id == id)
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
        }
    }
}