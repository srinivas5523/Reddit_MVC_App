using Newtonsoft.Json;

namespace Reddit_MVC_App.Models
{
    public class UsersListing
    {
        [JsonProperty("kind")]
        public string ? Kind { get; set; }
        [JsonProperty("data")]
        public ChildUser ? Data { get; set; }
      

    }

    public class ChildUser
    {
        [JsonProperty("after")]
        public string ?  After { get; set; }

        [JsonProperty("children")]
        public List<UserList>? Childern { get; set; }
    }

    public class UserList
    {
        [JsonProperty("kind")]
        public string ? Kind { get; set; }
        [JsonProperty("data")]
        public User? Data { get; set; }
    }

    public class User
    {
        [JsonProperty("display_name")]
        public string? DisplayName { get; set; }
        [JsonProperty("title")]
        public string? Title { get; set; }

    }
    
}
