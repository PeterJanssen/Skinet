using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Core.Paging
{
    public class Pagination<T> where T : class
    {
        public Pagination(int pageIndex, int pageSize, int count, IReadOnlyList<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }
        [JsonPropertyName("pageindex")]
        public int PageIndex { get; set; }
        [JsonPropertyName("pagesize")]
        public int PageSize { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("data")]
        public IReadOnlyList<T> Data { get; set; }
    }
}