using Newtonsoft.Json;

namespace Reddit_MVC_App.Models
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public required string ? AccessToken { get; set; }
        }
}