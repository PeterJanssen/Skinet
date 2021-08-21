using Domain.Models.BasketModels;
using System.Threading.Tasks;

namespace Persistence.Data.Repository.Interfaces
{
    public interface IBasketRepository
    {
        Task<CustomerBasket> GetBasketAsync(string basketId);
        Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket);
        Task<bool> DeleteBasketAsync(string basketId);

    }
}