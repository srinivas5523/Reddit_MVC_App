using Microsoft.AspNetCore.Mvc;
using Reddit_MVC_App.Models;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Mime;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections;
using System.Resources;
using System.Globalization;
using System;
using System.ComponentModel.Design;
using System.Configuration;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Hosting.Server;
using System.Xml.Linq;
using Reddit_MVC_App.Services;
using NLog;
using System.Reflection.Metadata;
using System.Data.Entity.Infrastructure;
using NuGet.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.CodeAnalysis.Elfie.Model.Strings;
using Newtonsoft.Json.Linq;
using Humanizer;
using System.Threading.RateLimiting;
using static NuGet.Client.ManagedCodeConventions;


namespace Reddit_MVC_App.Controllers
{
    public class HomeController : Controller
    {

        private readonly IConfiguration _iconfig;
        string _accessToken = string.Empty;
        private readonly IHttpClientHelper _httpClientHelper;

        RedditConfig myStronglyTypedModel = new RedditConfig();
        string _baseURL = Reddit_MVC_App.Models.RedditConfig._baseURL;
        string _clientId = Reddit_MVC_App.Models.RedditConfig._clientId;
        string _clientSecret = Reddit_MVC_App.Models.RedditConfig._clientSecret;
        string _oAuthURL = Reddit_MVC_App.Models.RedditConfig._oAuthURL;
        string _subReddit = Reddit_MVC_App.Models.RedditConfig._subReddit;
        string _userAgent = Reddit_MVC_App.Models.RedditConfig._userAgent;
        string _host = Reddit_MVC_App.Models.RedditConfig._host;
        string _getAccessToken = Reddit_MVC_App.Models.RedditConfig._getAccessToken;
        string _users = Reddit_MVC_App.Models.RedditConfig._users;
        string _user = Reddit_MVC_App.Models.RedditConfig._user;

        object AccessToken;
        private readonly ILoggerManager _logger;
        public string token = string.Empty;

        List<AboutUserDet> AboutUserDet = new List<AboutUserDet>();

      

        public HomeController(ILoggerManager logger, IConfiguration iconfig, IHttpClientHelper httpClientHelper)
        {
            _logger = logger;
            _iconfig = iconfig;
            this._httpClientHelper = httpClientHelper;
        }

        public async Task<IActionResult> Index()
        {
            DropdownViewModel model = new DropdownViewModel();
            model.SelectedCategoryID = "0";
            
            
            if (ModelState.IsValid)
            {
                var token = GetToken();
            }
            _logger.LogInfo(":: Startes Here ::");
            try
            {
                _logger.LogInfo(":: Start getting Token Details ::");
                _logger.LogInfo("::Completed GetToken Details Method::");
                this.ViewBag.CategoryList = this.GetCategoryList();
                return this.View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            // Info.
            return this.View(model);
            }

        private IEnumerable<SelectListItem> GetCategoryList()
        {
            // Initialization.
            SelectList lstobj = null;

            try
            {
                // Loading.
                var list = this.LoadData()
                                  .Select(p =>
                                            new SelectListItem
                                            {
                                                Value = p.Category_ID.ToString(),
                                                Text = p.CategoryName
                                            });

                // Setting.
                lstobj = new SelectList(list, "Value", "Text");
               
            }
            catch (Exception ex)
            {
                // Info
                _logger?.LogError("GetCategoryList" + ex.Message);
            }

            // info.
            return lstobj;
        }

        private List<Category> LoadData()
        {
            // Initialization.
            List<Category> lst = new List<Category>();

            try
            {
                // Initialization.
                string line = string.Empty;
                string srcFilePath = "content/files/Category.txt";
                var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                var fullPath = Path.Combine(rootPath, srcFilePath);
                string filePath = new Uri(fullPath).LocalPath;
                StreamReader sr = new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read));

                // Read file.
                while ((line = sr.ReadLine()) != null)
                {
                    // Initialization.
                    Category infoObj = new Category();
                    string[] info = line.Split(',');

                    // Setting.
                    infoObj.Category_ID = info[0].ToString();
                    infoObj.CategoryName = info[1].ToString();

                    // Adding.
                    lst.Add(infoObj);
                }
                // Closing.
                sr.Dispose();
                sr.Close();
            }
            catch (Exception ex)
            {
                // info.
                _logger.LogInfo("LoadData" + ex.Message);
            }

            // info.
            return lst;
        }

        public String GetToken()
        {
            string TokenValue = string.Empty;
            try
            {
                RateLimit rateLimit = new RateLimit();
                _logger.LogInfo("Calling generate token endpoint");
                TokenValue = _httpClientHelper.GetAsync(_baseURL + _getAccessToken, _oAuthURL, _getAccessToken, _clientId, _clientSecret, _subReddit, _host, _userAgent, "",out rateLimit);
                var ObjTokenResponse = JsonConvert.DeserializeObject<TokenResponse>(TokenValue);
                TokenValue = ObjTokenResponse.AccessToken;
                _logger.LogInfo(":: Token value ::" + TokenValue);

                if (ObjTokenResponse != null)
                {
                    HttpContext.Session.SetString("accessToken", TokenValue);

                }
                else { _logger.LogInfo("No Token Generated"); }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetToken:" + ex.Message);
            }
            return TokenValue;
        }    

        public ActionResult GetSubredditData(string Cname)
        {
            string SubredditData = string.Empty;
            string dataList = string.Empty;
            RateLimit rateLimit = new RateLimit();
            try
            {
                if (HttpContext.Session.GetString("accessToken") != null)
                {
                   dataList = _httpClientHelper.GetAsync(_baseURL, _oAuthURL + _subReddit + Cname + "/top?t=all&limit=5", HttpContext.Session.GetString("accessToken").ToString(), _clientId, _clientSecret, _subReddit, _host, _userAgent, Cname , out rateLimit);
                   rateLimit = rateLimit;

                   HttpContext.Session.SetString("RateLmtUsed", rateLimit.RatelimitUsed.ToString());
                   HttpContext.Session.SetString("RateLmtRemaining", rateLimit.RatelimitRemaing.ToString());
                   HttpContext.Session.SetString("RateLmtReset", rateLimit.RatelimitReset.ToString());
                   _logger.LogInfo("GetSubredditData :: RateLmtUsed ::" + rateLimit.RatelimitUsed.ToString());
                   _logger.LogInfo("GetSubredditData :: RateLmtRemaining ::" + rateLimit.RatelimitRemaing.ToString());
                   _logger.LogInfo("GetSubredditData :: RateLmtReset ::" + rateLimit.RatelimitReset.ToString());
                }
                else
                {
                    _logger.LogInfo("Token Value is Null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetSubredditData" + ex.Message);
            }
            RateLmt(rateLimit);
            return PartialView(JsonConvert.DeserializeObject<SubredditListing>(dataList));
            
        }
        public ActionResult GetUserDetail()
        {
            string dataList = string.Empty;
            string _dataList = string.Empty;
            string Pkarma = string.Empty;
            RateLimit rateLimit = new RateLimit();
            
            try
            {
                if (HttpContext.Session.GetString("accessToken") != null)
                {
                    dataList = _httpClientHelper.GetAsync(_baseURL, _oAuthURL + _users, HttpContext.Session.GetString("accessToken").ToString(), _clientId, _clientSecret, _subReddit, _host, _userAgent, "Usr",out rateLimit);
                    if (dataList != null)
                    {

                        foreach (var item in JsonConvert.DeserializeObject<UsersListing>(dataList).Data.Childern)
                        {
                            _logger.LogInfo("Display Name:" + item.Data.Title);
                            _logger.LogInfo("Display Name:" + item.Data.DisplayName);
                            
                            // passing author name get the title,display name and Total Karma
                            _dataList = _httpClientHelper.GetAsync(_baseURL, _oAuthURL + _user + (item.Data.DisplayName).Replace("u_", "") + "/about", HttpContext.Session.GetString("accessToken").ToString(), _clientId, _clientSecret, _subReddit, _host, _userAgent, "Usr", out RateLimit _ratelimit);
                            var AboutUserDetail = JsonConvert.DeserializeObject<AboutUsersListing>(_dataList).Data;
                            _logger.LogInfo("Total Karma:" + Convert.ToDouble(AboutUserDetail.TotalKarma));
                            AboutUserDet.Add(new AboutUserDet { title = AboutUserDetail.Subreddit.Title, display_name = AboutUserDetail.Subreddit.DisplayName.Replace("u_", ""), total_karma = Convert.ToDouble(AboutUserDetail.TotalKarma)});
                            rateLimit = _ratelimit;
                        }
                    }
                    HttpContext.Session.SetString("RateLmtUsed", rateLimit.RatelimitUsed.ToString());
                    HttpContext.Session.SetString("RateLmtRemaining", rateLimit.RatelimitRemaing.ToString());
                    HttpContext.Session.SetString("RateLmtReset", rateLimit.RatelimitReset.ToString());

                  //  return PartialView(AboutUserDet);
                }
                else
                {
                    _logger.LogInfo("Token Value is Null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetUserDetail" + ex.Message);
            }
            return PartialView(AboutUserDet);
            RateLmt(rateLimit);
        }

        public ActionResult RateLmt(RateLimit rateLimit)
        {
                rateLimit.RatelimitUsed = HttpContext.Session.GetString("RateLmtUsed");
                rateLimit.RatelimitRemaing = HttpContext.Session.GetString("RateLmtRemaining");
                rateLimit.RatelimitReset = HttpContext.Session.GetString("RateLmtReset");
                return PartialView(rateLimit);
        }
       }

}