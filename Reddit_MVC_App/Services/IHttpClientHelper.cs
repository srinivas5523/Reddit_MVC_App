using Reddit_MVC_App.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Reddit_MVC_App.Services
{

    public interface IHttpClientHelper
    {
        string GetAsync(string requestUri , string _oAuthURL, string getAccessToken, string _clientId, string _clientSecret , string _subReddit, string _host, string _userAgent , string _Type, out RateLimit? _RateLimit);
       
        HttpClient HttpClient { get; set; }

    }

}
