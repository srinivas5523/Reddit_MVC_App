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
        private readonly IHttpClientHelper _httpClientHelper;
        private readonly IRedditService _redditService;       
        private readonly ILoggerManager _logger;
        
       public HomeController(ILoggerManager logger, IConfiguration iconfig, IHttpClientHelper httpClientHelper, IRedditService redditService)
        {
            _logger = logger;
            //_iconfig = iconfig;
            this._httpClientHelper = httpClientHelper;
            _redditService = redditService;
        }

        public async Task<IActionResult> Index()
        {
            DropdownViewModel model = new DropdownViewModel();
            model.SelectedCategoryID = "0";

            _logger.LogInfo(":: Startes Here ::");      
            
            try
            {
                if (ModelState.IsValid)
                {
                    if (Authenticate()) 
                    {                        
                        this.ViewBag.CategoryList = _redditService.GetCategoryList();
                    }                  

                }     
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }            
            return this.View(model);
            }
             

        public ActionResult GetSubredditData(string Cname)
        {
            string _SubredditData = string.Empty;
            string dataList = string.Empty;
            RateLimit rateLimit = new RateLimit();
            try
            {
               
                if (!string.IsNullOrEmpty(AccessToken))
                {                    
                    _SubredditData = _redditService.GetSubRedditData(Cname, AccessToken, out rateLimit);
                    SetRateLimit(rateLimit);

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
            RateLmt();
            return PartialView(JsonConvert.DeserializeObject<SubredditListing>(_SubredditData));
            
        }
        public ActionResult GetUserDetail()
        {
            string dataList = string.Empty;            
            RateLimit rateLimit = new RateLimit();
            List<UserPostDetail> userRespData = new List<UserPostDetail>(); 

            try
            {
                if (!string.IsNullOrEmpty(AccessToken))
                {
                    
                    userRespData = _redditService.GetUserPostsData(AccessToken, out rateLimit);                    
                    SetRateLimit(rateLimit);
                    
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
            return PartialView(userRespData);
           
        }

        public ActionResult RateLmt()
        {
            RateLimit rateLimit = new RateLimit();
            try
            {
               rateLimit = GetRateLimit();

                _logger.LogInfo("Retrived Rate limit data.");
            }
            catch (Exception ex)
            {

                _logger.LogError("RateLmt" + ex.Message); 
            }
            
                return PartialView(rateLimit);
        }

        #region Helper Methods

        private bool Authenticate() 
        {
            bool result = false;
            try
            {
                _logger.LogInfo(":: Start getting Token Details ::");
                string token = _redditService.Authenticate();
                _logger.LogInfo("::Completed GetToken Details Method::");

                if (!string.IsNullOrEmpty(token))
                {
                    AccessToken = token;
                    string Token1 = AccessToken.ToString();
                   result = true;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            return result;
        }

        private void SetRateLimit(RateLimit rateLimit) 
        {
            try
            {
                RateLmtUsed = rateLimit.RatelimitUsed;
                RateLmtRemaining = rateLimit.RatelimitRemaing;
                RateLmtReset = rateLimit.RatelimitReset;
            }
            catch (Exception ex)
            {

                _logger.LogError("setRateLimit" + ex.Message);
            }
        }

        private RateLimit GetRateLimit()
        {
            RateLimit rateLimit = new RateLimit();
            try
            {
               rateLimit.RatelimitUsed = RateLmtUsed;
                rateLimit.RatelimitRemaing = RateLmtRemaining;
                rateLimit.RatelimitReset = RateLmtReset; 
            }
            catch (Exception ex)
            {

                _logger.LogError("setRateLimit" + ex.Message);
            }
            return  rateLimit;
        }


        #endregion

        #region sessionData
        private string AccessToken   // property
        {
            get { return HttpContext.Session.GetString("accessToken"); }   // get method
            set { HttpContext.Session.SetString("accessToken", value); }  // set method
        }

        private string RateLmtUsed   // property
        {
            get { return HttpContext.Session.GetString("RateLmtUsed"); }   // get method
            set { HttpContext.Session.SetString("RateLmtUsed", value); }  // set method
        }

        private string RateLmtRemaining   // property
        {
            get { return HttpContext.Session.GetString("RateLmtRemaining"); }   // get method
            set { HttpContext.Session.SetString("RateLmtRemaining", value); }  // set method
        }

        private string RateLmtReset   // property
        {
            get { return HttpContext.Session.GetString("RateLmtReset"); }   // get method
            set { HttpContext.Session.SetString("RateLmtReset", value); }  // set method
        }
        #endregion



    }

}