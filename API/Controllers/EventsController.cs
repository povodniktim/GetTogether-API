using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
