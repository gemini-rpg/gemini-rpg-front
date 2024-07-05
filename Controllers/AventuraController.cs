using AiRpgFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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


        public IActionResult Index(string nome)
        {

            ViewBag.Nome = nome;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarHistoria(PersonagemViewModel model)
        {
            try
            {
                string data = JsonConvert.SerializeObject(model);
                StringContent conteudo = new(data, Encoding.UTF8, "application/json");
                HttpResponseMessage resposta = await _client.PostAsync(_client.BaseAddress + "/create_session", conteudo);
                if (resposta.IsSuccessStatusCode)
                {
                    string result = resposta.Content.ReadAsStringAsync().Result;
                    HistoriaViewModel historia = JsonConvert.DeserializeObject<HistoriaViewModel>(result)!;
                    ViewBag.Historia = historia.Historia;
                    TempData["Historia"] = historia.Historia;
                    ViewBag.Nome = model.nome;
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro";
            }
            return View("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PostSpeech()
        {
            // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
            string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
            string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

            string text = TempData["Historia"].ToString();

            try
            {
                var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
                speechConfig.SpeechSynthesisVoiceName = "pt-BR-ElzaNeural";

                using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
                {
                    var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(text);

                    // OutputSpeechSynthesisResult(speechSynthesisResult, text); // Pode não ser necessário para o retorno HTTP

                    // Verifica o resultado da síntese de fala
                    if (speechSynthesisResult.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        // Se a síntese foi completada com sucesso, retornar um OkResult
                        return View();
                    }
                    else
                    {
                        // Se houve cancelamento ou erro na síntese
                        var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                        return BadRequest($"Speech synthesis failed. Reason={cancellation.Reason}, ErrorCode={cancellation.ErrorCode}, ErrorDetails={cancellation.ErrorDetails}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Captura de exceções gerais
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
