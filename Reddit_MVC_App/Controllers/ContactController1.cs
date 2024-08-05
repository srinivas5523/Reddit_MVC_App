using Microsoft.AspNetCore.Mvc;

namespace Reddit_MVC_App.Controllers
{
    public class ContactController1 : Controller
    {
        public IActionResult Index()
        {
            return View("Privacy");
        }
    }
}
