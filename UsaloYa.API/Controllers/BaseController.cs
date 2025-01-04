using Microsoft.AspNetCore.Mvc;

namespace UsaloYa.API.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
