using Microsoft.AspNetCore.Mvc;

namespace AiRpgFrontEnd.Controllers
{
    public class AventuraController : Controller
    {
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
        {
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
