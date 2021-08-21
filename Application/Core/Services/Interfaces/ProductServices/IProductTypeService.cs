using Domain.Models.ProductModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Core.Services.Interfaces.ProductServices
{
    public interface IProductTypesService
    {
        Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
    }
}