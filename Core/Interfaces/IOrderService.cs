using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using Core.Specifications;

namespace Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, int deliverMethod, string basketId, Address shippingAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(OrderSpecParams specParams, string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
        Task<int> CountProductsAsync(OrderSpecParams specParams, string email);
    }
}