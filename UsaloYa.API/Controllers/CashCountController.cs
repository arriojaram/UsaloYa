using Microsoft.AspNetCore.Mvc;

namespace UsaloYa.API.Controllers
{
    public class CashCountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
