using Microsoft.AspNetCore.Mvc.Rendering;
using Reddit_MVC_App.Models;

namespace Reddit_MVC_App.Services
{
    public interface IRedditService
    {
        string Authenticate();

        bool IsAccessTokenExists();

        string GetSubRedditData(string subReditType, string accessToken, out RateLimit rateLimit);

        List<UserPostDetail> GetUserPostsData(string accessToken, out RateLimit rateLimit);

        SelectList GetCategoryList();


    }
}
