﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Reddit_MVC_App.Models;
using System.Threading.RateLimiting;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace Reddit_MVC_App.Services
{
    
    public class RedditService : PageModel, IRedditService
    {

        private readonly ILoggerManager _logger;
        private readonly IConfiguration _iconfig;
        private readonly IHttpClientHelper _httpClientHelper;        
        private string _baseURL, _clientId, _clientSecret, _oAuthURL, _subReddit, _userAgent, _host, _accessToken, _users, _user;
       

        public RedditService( ILoggerManager logger, IConfiguration iconfig, IHttpClientHelper httpClientHelper )
        {
            _logger = logger;
            _iconfig = iconfig;
            this._httpClientHelper = httpClientHelper;
            this.GetConfigData();
        }
       
        public string Authenticate() 
        {
            string _accessToken = string.Empty;
            try
            {
              
                string _requestURI = _baseURL + this._accessToken;

                _logger.LogInfo("Calling HTTPClient calss to get access token");
                string _tokenResponse = _httpClientHelper.generateAuthToken(_clientId, _clientSecret, _requestURI, _host,_userAgent);
                var ObjTokenResponse = JsonConvert.DeserializeObject<TokenResponse>(_tokenResponse);

                if (ObjTokenResponse != null &&  !string.IsNullOrEmpty(ObjTokenResponse.AccessToken))
                {
                    _accessToken = ObjTokenResponse.AccessToken;               
                    _logger.LogInfo("Credentials authenticated. Access token generated, Token availble in Session." );

                }
                else { _logger.LogInfo("No Token Generated"); }

            }
            catch (Exception ex)
            {

                _logger.LogError("Authenticate:" + ex.Message);
            }
            return _accessToken;
        }
        
        public bool IsAccessTokenExists() 
        {
            bool isToken = false;
            try
            {

                if (!string.IsNullOrEmpty(HttpContext.Session.GetString("accessToken")))                    
                {
                    isToken = true;
                    _logger.LogInfo("Access Token Exists");
                }
                else
                {
                    _logger.LogInfo("No Access Token Exists");
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("isAuthenticationTokenExists:" + ex.Message);
            }    
            return isToken; 
        }

       public string GetSubRedditData(string subRedditType, string accessToken, out RateLimit rateLimit) 
        {
            string SubredditData = string.Empty;
            string requestURL = string.Empty;
            string subRedditData = string.Empty;
            RateLimit _rateLimit = new RateLimit();

            try
            {
                requestURL = _oAuthURL + _subReddit + subRedditType + "/top?t=all&limit=5"; //<TD:Re work on limit to retrive from config>
                if (!string.IsNullOrEmpty(accessToken))
                {
                    subRedditData = _httpClientHelper.getRedittData(accessToken, requestURL, _userAgent, out _rateLimit);                    

                }
                else
                {
                    _logger.LogInfo("Access Token null (or) not exists");
                }

            }
            catch (Exception ex)
            {

                _logger.LogError("getSubRedditData:" + ex.Message);
            }
            rateLimit = _rateLimit;
            return subRedditData; 
        }

        //Get User Details
        public List<UserPostDetail> GetUserPostsData(string accessToken, out RateLimit rateLimit)
        {
            string usersList = string.Empty;
            string _dataList = string.Empty;
            string Pkarma = string.Empty;
            RateLimit _rateLimit = new RateLimit();
            List<UserPostDetail> UsersPostsList = new List<UserPostDetail>();

            try
            {
                string _requestURL = string.Empty;
                _requestURL = _oAuthURL + _users;               

                if (!string.IsNullOrEmpty(accessToken))
                {
                    usersList = _httpClientHelper.getRedittData(accessToken, _requestURL, _userAgent, out _rateLimit);

                    if (!string.IsNullOrEmpty(usersList)) 
                    {
                        foreach (var item in JsonConvert.DeserializeObject<UsersListing>(usersList).Data.Childern)
                        {
                            _logger.LogInfo("Display Title:" + item.Data.Title);
                            _logger.LogInfo("Display Name:" + item.Data.DisplayName);

                            // passing author name get the title,display name and Total posts (Karma)
                            _requestURL = _oAuthURL + _user + (item.Data.DisplayName).Replace("u_", "") + "/about";
                            _dataList = _httpClientHelper.getRedittData(accessToken, _requestURL, _userAgent, out _rateLimit);

                            var UserPostDetail = JsonConvert.DeserializeObject<AboutUsersListing>(_dataList).Data;

                            _logger.LogInfo("Total Karma:" + Convert.ToDouble(UserPostDetail.TotalKarma));

                            UsersPostsList.Add(new UserPostDetail { title = UserPostDetail.Subreddit.Title, display_name = UserPostDetail.Subreddit.DisplayName.Replace("u_", ""), total_karma = Convert.ToDouble(UserPostDetail.TotalKarma) });
                           
                        }
                    }
                    else
                    {
                        _logger.LogInfo("Users data not exists");
                    }                      
                }
                else
                {
                    _logger.LogInfo("Access Token null (or) not exists");
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("getSubRedditData:" + ex.Message);
            }
            rateLimit = _rateLimit;
            return UsersPostsList; 
        }       

         public SelectList GetCategoryList()
         {

            SelectList _catList = null;
            try
            {
                List<Category> lst = new List<Category>() {
                                         new Category { Category_ID = "Gaming", CategoryName= "Gaming" },
                                         new Category { Category_ID = "Music", CategoryName= "Music" },
                                         new Category { Category_ID = "Technology", CategoryName= "Technology" },
                                         new Category { Category_ID = "Art", CategoryName= "Art" },
                                         new Category { Category_ID = "IT", CategoryName= "IT" } }; //<TD Use Reddit end point to get sub catgeoriest>

                _catList = new SelectList(lst.Select(p =>
                                            new SelectListItem
                                            {
                                                Value = p.Category_ID.ToString(),
                                                Text = p.CategoryName
                                            }),"Value", "Text");

                _logger.LogInfo("Redditt SUbcategory list loaded for user selection.");
            }
            catch (Exception ex)
            {

                _logger.LogError("GetCategoryList:" + ex.Message);
            }

            return _catList;
         }

        private void GetConfigData()
        {
            try
            {
                _logger.LogInfo("Load Config data.");
                _baseURL = _iconfig["RedditUrl:baseUrl"].ToString();
                _clientId = _iconfig["Credentials:clientId"].ToString();
                _clientSecret = _iconfig["Credentials:clientSecret"].ToString();
                _oAuthURL = _iconfig["RedditUrl:oAuthUrl"].ToString();
                _subReddit = _iconfig["subReddit"].ToString();
                _userAgent = _iconfig["userAgent"].ToString(); ;
                _host = _iconfig["RedditUrl:host"].ToString();
                _accessToken = _iconfig["RedditUrl:tokenUrl"].ToString();
                _users = _iconfig["RedditUrl:users"].ToString();
                _user = _iconfig["user"].ToString();
                _logger.LogInfo("Loaded Config data.");
            }
            catch (Exception ex)
            {
                _logger.LogError("GetConfigData:" + ex.Message);
                throw ex;
            }

        }




    }
}
