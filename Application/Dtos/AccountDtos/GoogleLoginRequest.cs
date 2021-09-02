using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Dtos.AccountDtos
{
    public class GoogleLoginRequest
    {
        [Required]
        [JsonPropertyName("provider")]
        public string Provider { get; set; }
        [Required]
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }
    }
}
