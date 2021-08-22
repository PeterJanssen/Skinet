using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Application.Dtos.Product
{
    public class ProductToReturnDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("pictureUrl")]
        public string PictureUrl { get; set; }
        [JsonPropertyName("ProductType")]
        public string ProductType { get; set; }
        [JsonPropertyName("productBrand")]
        public string ProductBrand { get; set; }
        [JsonPropertyName("photos")]
        public IEnumerable<PhotoToReturnDto> Photos { get; set; }
        [JsonPropertyName("reviews")]
        public IEnumerable<ProductReviewDto> Reviews { get; set; }
    }
}