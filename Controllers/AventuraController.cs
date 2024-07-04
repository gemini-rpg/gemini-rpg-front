using Microsoft.AspNetCore.Mvc;

namespace AiRpgFrontEnd.Controllers
{
    public class AventuraController : Controller
    {
<<<<<<< HEAD
        [HttpPost]
        public IActionResult Index(string nome)
=======
        private readonly Uri apiUri = new("http://127.0.0.1:5000");
        private readonly HttpClient _client;

        public AventuraController ()
        {
            _client = new HttpClient
            {
                BaseAddress = apiUri
            };
        }

        public IActionResult Index()
>>>>>>> d3215d21981a666c9551fd4a7ec50cdb57c8ebea
        {
            ViewBag.Nome = nome;
            return View();
        }

        [HttpPost]
        public async Task<string> CriarHistoria()
        {
            HttpResponseMessage resposta = await _client.GetAsync(apiUri + "/create_session");

            return "";
        }
    }
}
