using System.Text.Json.Serialization;

namespace Application.Dtos.ProductDtos
{
    public class ProductBrandDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}