using Microsoft.AspNetCore.Mvc;

namespace AiRpgFrontEnd.Controllers
{
    public class PersonagemController : Controller
    {
        private readonly ILogger<PersonagemController> _logger;

        public PersonagemController(ILogger<PersonagemController> logger)
        {
            _logger = logger;
        }

       
        public IActionResult Index()
        {
            return View();
        }
    }
}
