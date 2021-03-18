using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
    public class OrderWithFiltersForCountSpecification : BaseSpecification<Order>
    {
        public OrderWithFiltersForCountSpecification(OrderSpecParams specParams, string email)
         : base(o => o.BuyerEmail == email && o.Status == (OrderStatus)specParams.Status)
        {

        }
    }
}