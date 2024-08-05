using Newtonsoft.Json;

namespace Reddit_MVC_App.Models
{
    public class AboutUsersListing
    {
        [JsonProperty("kind")]
        public string ? Kind { get; set; }
        [JsonProperty("data")]
        public UsrChilddata  ? Data { get; set; }
    }

    public class UsrChilddata
    {
        [JsonProperty("total_karma")]
        public double ? TotalKarma { get; set; }
        [JsonProperty("subreddit")]
        public UsrSubReddit ? Subreddit { get; set; }            
    }
    public class UsrSubReddit
    {
        [JsonProperty("display_name")]
        public string ? DisplayName { get; set; }
        [JsonProperty("title")]
        public string ? Title { get; set; }
    }
   
}
