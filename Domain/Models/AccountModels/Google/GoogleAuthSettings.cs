using System.Text.Json.Serialization;

namespace Domain.Models.AccountModels.Google
{
    public class GoogleAuthSettings
    {
        [JsonPropertyName("clientId")]
        public string ClientId { get; set; }
    }
}
