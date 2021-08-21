using System.Text.Json.Serialization;

namespace Application.Dtos.AccountDtos
{
    public class ImpersonationRequest
    {
        [JsonPropertyName("username")]
        public string UserName { get; set; }
    }
}
