using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class ProductsWithTypesAndBrandsSepcification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSepcification()
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
        }

        public ProductsWithTypesAndBrandsSepcification(int id) : base(product => product.Id == id)
        {
            AddInclude(product => product.ProductType);
            AddInclude(product => product.ProductBrand);
        }
    }
}