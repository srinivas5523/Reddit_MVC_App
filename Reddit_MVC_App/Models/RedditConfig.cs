namespace Reddit_MVC_App.Models
{
    public class RedditConfig
    {
        public static string _baseURL 
        {
        get { return "https://www.reddit.com/"; }
        }
        public static string _oAuthURL
        {
            get { return "https://oauth.reddit.com/"; }
        }
        public static string _host
        {
            get { return "www.reddit.com"; }
        }
        public static string _userAgent
        {
            get { return "Reddit Challenge"; }
        }
        public static string _clientId
        {
            get { return "e4N9SKyfIYFBAKM5fynOUw"; }
        }
        public static string _clientSecret
        {
            get { return "s6Y0jc_AOG4AZWgwA_EAaAyQQHUW2A"; }
        }
        public static string _subReddit
        {
            get { return "r/"; }
        }
        public static string _getAccessToken
        {
            get { return "api/v1/access_token?grant_type=client_credentials"; }
        }

        public static string _users
        {
            get { return "users?t=all&limit=5"; }
        }
        public static string _user
        {
            get { return "user/"; }
        }

        public string AccessToken { get; set; }



    }
}
