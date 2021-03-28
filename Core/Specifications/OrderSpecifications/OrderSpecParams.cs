namespace Core.Specifications.OrderSpecifications
{
    public class OrderSpecParams
    {
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
        public string Sort { get; set; }
        public int Status { get; set; }
        private string _search;
        public string Search { get => _search; set => _search = value.ToLower(); }
    }
}