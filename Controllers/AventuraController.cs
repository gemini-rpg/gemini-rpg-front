using Microsoft.AspNetCore.Mvc;

namespace AiRpgFrontEnd.Controllers
{
    public class AventuraController : Controller
    {
        [HttpPost]
        public IActionResult Index(string nome)
        {
            ViewBag.Nome = nome;
            return View();
        }
    }
}
