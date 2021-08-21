using Domain.Models.ProductModels;
using Persistence.Data.Repository;

namespace Application.Core.Specifications.ProductSpec
{
    public class ProductsWithTypesAndBrandsSpecification : Specification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(ProductSpecParams specParams)
         : base(product =>
                (string.IsNullOrEmpty(specParams.Search) || product.Name.ToLower().Contains(specParams.Search)) &&
                (!specParams.BrandId.HasValue || product.ProductBrandId == specParams.BrandId) &&
                (!specParams.TypeId.HasValue || product.ProductTypeId == specParams.TypeId)
                )
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
            AddInclude(product => product.Reviews);
            AddInclude(x => x.Photos);
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

        public ProductsWithTypesAndBrandsSpecification(int id) : base(product => product.Id == id)
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
            AddInclude(product => product.Reviews);
            AddInclude(x => x.Photos);
        }
    }
}