using Microsoft.AspNetCore.Mvc;

namespace SFBlog.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
