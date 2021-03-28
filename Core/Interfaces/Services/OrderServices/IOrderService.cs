using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.BasketEntities;
using Core.Entities.OrderEntities;
using Core.Specifications.OrderSpecifications;

namespace Core.Interfaces.Services.OrderServices
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, DeliveryMethod deliveryMethod, CustomerBasket customerBasket, OrderAddress shippingAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(OrderSpecParams specParams, string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
        Task<int> CountProductsAsync(OrderSpecParams specParams, string email);
    }
}