using AiRpgFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace AiRpgFrontEnd.Controllers
{
    public class AventuraController : Controller
    {

        private readonly Uri apiUri = new("http://127.0.0.1:5000");
        private readonly HttpClient _client;

        public AventuraController()
        {
            _client = new HttpClient
            {
                BaseAddress = apiUri
            };
        }


        [HttpPost]
        public IActionResult Index(string nome)

        {
            ViewBag.Nome = nome;
            return View();
        }

        [HttpPost]
        public async Task<string> CriarHistoria(PersonagemViewModel model)
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent conteudo = new StringContent(data, Encoding.UTF8, "application/json");
           

                HttpResponseMessage resposta = await _client.PostAsync(apiUri + "/create_session", conteudo);

                if (resposta.IsSuccessStatusCode)
                {
                    return "";
                }

            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro";
                
            }
           

            return "";
        }
    }
}
