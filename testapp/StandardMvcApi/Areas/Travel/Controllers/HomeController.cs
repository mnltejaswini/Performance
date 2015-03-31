using Microsoft.AspNet.Mvc;

namespace StandardMvcApi.Areas.Travel.Controllers
{
    /// <summary>
    /// There is no performance test scenario runs through this contoller yet. The solo purpose of this controller
    /// is to fill out the routing/action-selection tables.
    /// </summary>
    [Area("Travel")]
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return Content("This is the Travel/Home/Index action");
        }
    }
}
