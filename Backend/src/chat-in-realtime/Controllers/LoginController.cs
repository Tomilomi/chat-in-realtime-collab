using Microsoft.AspNetCore.Mvc;

namespace chat_in_realtime.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}