using System.Linq;
using Domain.Models.OrderModels;
using Persistence.Data.Repository;

namespace Application.Core.Specifications.OrderSpec
{
    public class OrderWithFiltersForCountSpecification : Specification<Order>
    {
        public OrderWithFiltersForCountSpecification(OrderSpecParams specParams, string email) : base(
            o => o.BuyerEmail == email &&
            (string.IsNullOrEmpty(specParams.Search) ||
             o.OrderItems.ToList().Any(orderItem => orderItem.ItemOrdered.ProductName.ToLower().Contains(specParams.Search.ToLower()))) &&
            o.Status == (OrderStatus)specParams.Status
            )
        {

        }
    }
}