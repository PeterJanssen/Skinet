namespace Application.Core.Specifications.OrderSpec
{
    public class OrderSpecParams : BaseSpecParams
    {
        private new int _pageSize = 10;
        public int Status { get; set; }
    }
}