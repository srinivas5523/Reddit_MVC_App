using Reddit_MVC_App.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Reddit_MVC_App.Services
{

    public interface IHttpClientHelper
    {
        
        public string generateAuthToken(string clientId, string clientSecret, string requestUri, string host, string userAgent);

        public string getRedittData(string accessToken, string requestUri, string userAgent, out RateLimit rateLimit);
        public HttpClient HttpClient { get; set; }

    }

}
