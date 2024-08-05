using Newtonsoft.Json;

namespace Reddit_MVC_App.Models
{
    public class AboutUserDet
    {

        [JsonProperty("DisplayName")]
        public string ? display_name { get; set; }
        [JsonProperty("Title")]
        public string ? title { get; set; }
        [JsonProperty("TotalKarma")]
        public double ? total_karma { get; set; }
       }
}
