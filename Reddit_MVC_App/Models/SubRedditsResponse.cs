using Newtonsoft.Json;

namespace Reddit_MVC_App.Models
{
    public class SubredditListing
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }
        [JsonProperty("data")]
        public ChildPost ? Data { get; set; }
    }

    public class ChildPost
    {
        [JsonProperty("after")]
        public string ?After { get; set; }
        [JsonProperty("children")]
        public List<SubredditList>? Childern { get; set; }
    }

    public class SubredditList
    {
        [JsonProperty("kind")]
        public string? Kind { get; set; }
        [JsonProperty("data")]
        public Subreddit? Data { get; set; }
    }

    public class Subreddit
    {
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("ups")]
        public int?  UPS { get; set; }
        [JsonProperty("author")]
        public string? Author { get; set; }
    }
}
