using System.Text.Json.Serialization;

namespace Application.Dtos.AccountDtos
{
    public class RefreshTokenRequest
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }
    }
}
