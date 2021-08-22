using System.Text.Json.Serialization;

namespace Domain.Models.ProductModels
{
    public class ProductType : BaseModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}