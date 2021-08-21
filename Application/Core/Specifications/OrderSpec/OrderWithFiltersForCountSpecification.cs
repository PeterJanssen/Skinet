using Domain.Models.OrderModels;
using Persistence.Data.Repository;

namespace Application.Core.Specifications.OrderSpec
{
    public class OrderWithFiltersForCountSpecification : Specification<Order>
    {
        public OrderWithFiltersForCountSpecification(OrderSpecParams specParams, string email)
         : base(o => o.BuyerEmail == email && o.Status == (OrderStatus)specParams.Status)
        {

        }
    }
}