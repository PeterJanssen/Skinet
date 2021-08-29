using System.ComponentModel.DataAnnotations;

namespace Application.Core.Specifications
{
    public class BaseSpecParams
    {
        [Range(1, int.MaxValue)]
        protected int MaxPageSize { get; set; } = 50;
        [Range(1, int.MaxValue)]
        public int PageIndex { get; set; } = 1;
        protected int _pageSize = 6;

        [Range(1, int.MaxValue)]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
        public string Sort { get; set; }
        protected string _search;
        public string Search { get => _search; set => _search = value.ToLower(); }
    }
}