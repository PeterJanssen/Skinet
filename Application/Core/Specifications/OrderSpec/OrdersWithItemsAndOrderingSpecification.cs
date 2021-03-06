using System.Linq;
using Domain.Models.OrderModels;
using Persistence.Data.Repository;

namespace Application.Core.Specifications.OrderSpec
{
    public class OrdersWithItemsAndOrderingSpecification : Specification<Order>
    {
        public OrdersWithItemsAndOrderingSpecification(OrderSpecParams specParams, string email) : base(
            o => o.BuyerEmail == email &&
            (string.IsNullOrEmpty(specParams.Search) ||
             o.OrderItems.ToList().Any(orderItem => orderItem.ItemOrdered.ProductName.ToLower().Contains(specParams.Search.ToLower()))) &&
            o.Status == (OrderStatus)specParams.Status
            )
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliverMethod);
            AddOrderBy(o => o.OrderDate);

            ApplyPaging(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "OrderDateAsc":
                        AddOrderBy(o => o.OrderDate);
                        break;
                    case "OrderDateDesc":
                        AddOrderByDescending(o => o.OrderDate);
                        break;
                    case "OrderPriceAsc":
                        AddOrderBy(o => o.SubTotal + o.DeliverMethod.Price);
                        break;
                    case "OrderPriceDesc":
                        AddOrderByDescending(o => o.SubTotal + o.DeliverMethod.Price);
                        break;
                    default:
                        AddOrderBy(o => o.OrderDate);
                        break;
                }
            }
        }

        public OrdersWithItemsAndOrderingSpecification(int id, string email)
         : base(o => o.Id == id && o.BuyerEmail == email)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliverMethod);
        }
    }
}