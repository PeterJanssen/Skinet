using System.Text.Json.Serialization;

namespace Application.Dtos.ProductDtos
{
    public class ProductTypeDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}