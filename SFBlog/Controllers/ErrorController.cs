using Microsoft.AspNetCore.Mvc;

namespace SFBlog.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Oops()
        {
            return View();
        }
    }
}
