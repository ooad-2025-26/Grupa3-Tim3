using Microsoft.AspNetCore.Mvc;

namespace FitManager.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
