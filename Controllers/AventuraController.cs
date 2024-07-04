using Microsoft.AspNetCore.Mvc;

namespace AiRpgFrontEnd.Controllers
{
    public class AventuraController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
