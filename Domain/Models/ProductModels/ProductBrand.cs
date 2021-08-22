using System.Text.Json.Serialization;

namespace Domain.Models.ProductModels
{
    public class ProductBrand : BaseModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}