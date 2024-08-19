using Newtonsoft.Json;
using Reddit_MVC_App.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NuGet.Protocol.Core.Types;
using System.Threading.RateLimiting;

namespace Reddit_MVC_App.Services
{
    public class HttpClientHelper : IHttpClientHelper
    {
        RedditConfig myRedditConfig = new RedditConfig();
        private readonly ILoggerManager _logger;
        public HttpClient HttpClient { get; set; }        

        public string generateAuthToken(string clientId, string clientSecret, string requestUri, string host, string userAgent) 
        {
            string _tokenResponse = string.Empty;
            try
            {
                string _baseUrl = string.Empty;
                string _responseType = string.Empty;
                using (var client = this.GetHttpClient()) 
                {
                    //Construct Request as per the Reditt guidelines. 
                    _baseUrl = $"{requestUri}";
                    client.DefaultRequestHeaders.Authorization = new  AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));
                    client.DefaultRequestHeaders.Add("Host", host);
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    var request = new HttpRequestMessage(HttpMethod.Post, _baseUrl);

                    using (var response = client.Send(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            _tokenResponse = response.Content.ReadAsStringAsync().Result;
                            
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
                return _tokenResponse;
            }

            return _tokenResponse; 
        }

        public string getRedittData(string accessToken,string requestUri, string userAgent, out RateLimit rateLimit)
        {
            string apiResponse = string.Empty;
            RateLimit _rateLimit = new RateLimit();
            try
            {
                string _baseUrl = string.Empty;
                string _responseType = string.Empty;                
                using (var client = this.GetHttpClient())
                {
                    //Construct http request as per the Reditt documenation guidelines. 

                    _baseUrl = $"{requestUri}";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);                    
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    var request = new HttpRequestMessage(HttpMethod.Get, _baseUrl);

                    using (var response = client.Send(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            apiResponse = response.Content.ReadAsStringAsync().Result;                            

                            //Get Api call statisitcs for Analysis.
                            _rateLimit.RatelimitUsed = response.Headers.GetValues("x-ratelimit-used").FirstOrDefault();
                            _rateLimit.RatelimitRemaing = response.Headers.GetValues("x-ratelimit-remaining").FirstOrDefault();
                            _rateLimit.RatelimitReset = response.Headers.GetValues("x-ratelimit-reset").FirstOrDefault();

                            rateLimit = _rateLimit;
                        }
                        else
                        {
                            throw new Exception($"Failed to retrieve Reditt posts data. Status code: {response.StatusCode}");
                        }
                    }


                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                
            }
            rateLimit = _rateLimit;
            return apiResponse;
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
