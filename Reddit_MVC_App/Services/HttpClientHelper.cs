using Newtonsoft.Json;
using Reddit_MVC_App.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Reddit_MVC_App.Services
{
    public class HttpClientHelper : IHttpClientHelper
    {
        RedditConfig myRedditConfig = new RedditConfig();
        private readonly ILoggerManager _logger;
        public HttpClient HttpClient { get; set; }

        public  string GetAsync(string requestUri, string _oAuthURL, string getAccessToken, string _clientId, string _clientSecret, string _subReddit, string _host, string _userAgent, string _Type, out RateLimit rateLimit)
        {
            string accessToken = string.Empty;
            HttpMethod _requestMethod = null;
            string _responseType = null;    
            RateLimit _rateLimit = new RateLimit();
            
            try
            {
                string _baseUrl;

                using (var client = this.GetHttpClient())
                {
                    if (_Type.Length > 0)
                    {
                        _baseUrl = $"{_oAuthURL}";
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", getAccessToken);
                        _requestMethod = HttpMethod.Get;
                    }
                    else
                    {
                        _baseUrl = $"{requestUri}";
                        client.DefaultRequestHeaders.Authorization = new
                 AuthenticationHeaderValue("Basic",
                 Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{_clientId}:{_clientSecret}")));

                        client.DefaultRequestHeaders.Add("Host", _host);
                        _requestMethod = HttpMethod.Post;

                    }
                    client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
                    var request = new HttpRequestMessage(_requestMethod, _baseUrl);
                    using (var response = client.Send(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            if (_Type.Length > 0)
                            {
                                _rateLimit.RatelimitUsed = response.Headers.GetValues("x-ratelimit-used").FirstOrDefault();
                                _rateLimit.RatelimitRemaing =response.Headers.GetValues("x-ratelimit-remaining").FirstOrDefault();
                                _rateLimit.RatelimitReset = response.Headers.GetValues("x-ratelimit-reset").FirstOrDefault();
                            }

                            var contentString = response.Content.ReadAsStringAsync();
                            _responseType = contentString.Result;
                            rateLimit = _rateLimit;
                            return _responseType;
                        }
                        else
                        {
                            throw new Exception($"Failed to retrieve access token. Status code: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                rateLimit = _rateLimit;
                return _responseType;
            }
        }
        private HttpClient GetHttpClient()
        {
            try
            {
                if (HttpClient == null)
                {
                    var _httpClient = new HttpClient();
                    _httpClient.DefaultRequestHeaders.Accept.Clear();
                    _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    return _httpClient;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return HttpClient;
        }

    
    }
}
