using Microsoft.AspNetCore.Mvc;

namespace PerfSample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(int id)
        {
            return View();
        }
    }
}