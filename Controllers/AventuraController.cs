using AiRpgFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json.Serialization;

namespace AiRpgFrontEnd.Controllers
{
    public class AventuraController : Controller
    {

        private readonly Uri apiUri = new("http://127.0.0.1:5000");
        private static string speechKey = "a46b765bbdcf4bc29363c79159952404";
        private static string speechRegion = "brazilsouth";
        private readonly HttpClient _client;
        private static readonly SpeechSynthesizer fala = new(SpeechConfig.FromSubscription(speechKey, speechRegion));
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
        public async Task<List<string>> ContinuarHistoria(Guid id, string nome, string escolha)
        {
            try
            {
                HistoriaViewModel historiaObject = new(id, escolha);
                string data = JsonConvert.SerializeObject(historiaObject);
                StringContent conteudo = new(data, Encoding.UTF8, "application/json");
                HttpResponseMessage resposta = await _client.PostAsync(_client.BaseAddress + "/chat", conteudo);
                List<string> continuacao = [];

                if (resposta.IsSuccessStatusCode)
                {
                    string result = resposta.Content.ReadAsStringAsync().Result;
                    HistoricoViewModel historia = JsonConvert.DeserializeObject<HistoricoViewModel>(result)!;
                    var hist = historia.message.Last().message;
                    continuacao.Add(FormataHistoria(hist));
                    if (hist.IndexOf("FIM") is not -1)
                    {
                        continuacao[0] = continuacao[0] + " FIM.";
                        return continuacao;

                    }
                    List<string> opcoes = FormataOpcoes(hist);
                    foreach (string opcao in opcoes)
                        continuacao.Add(opcao);
                    return continuacao;
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Erro";
            }
            return [""];
        }

        [HttpPost]
        public async Task<IActionResult> CriarHistoria(PersonagemViewModel model)
        {
            try
            {
                //string teste = "A nevoa da manha se dissipa lentamente, revelando a trilha tortuosa que se estende por entre as arvores imponentes da Floresta de Aethel. Voce, Gabriel, o mago, sente o cheiro umido da terra e a brisa fresca que acaricia seu rosto. As folhas das arvores, salpicadas de orvalho, refletem a luz fraca do sol nascente, criando um cenario mistico e encantador. Um fio de fumaca, quase imperceptivel, danca no horizonte, a oeste. Voce, com a sua visao agucada de mago, consegue identificar a origem: uma pequena cabana, quase escondida entre as arvores, com uma fumaca fina saindo de sua chamine. A trilha, que antes era clara, se divide em tres caminhos distintos: um que continua pela floresta, outro que se dirige para o rio que corta a mata e o ultimo que leva ate a cabana. --- Escolha uma acao: |1. Seguir o caminho pela floresta.| |2. Explorar o rio.| |3. Investigar a cabana.| ---";
                //string historia_formatada = FormataHistoria(teste);
                //List<string> opcoes = FormataOpcoes(teste);
                string data = JsonConvert.SerializeObject(model);
                StringContent conteudo = new(data, Encoding.UTF8, "application/json");
                HttpResponseMessage resposta = await _client.PostAsync(_client.BaseAddress + "/create_session", conteudo);
                if (resposta.IsSuccessStatusCode)
                {
                    string result = resposta.Content.ReadAsStringAsync().Result;
                    HistoriaViewModel historia = JsonConvert.DeserializeObject<HistoriaViewModel>(result)!;

                    var hist = historia.message;
                    ViewBag.Historia = FormataHistoria(hist);
                    TempData["Historia"] = ViewBag.Historia;
                    ViewBag.Opcoes = FormataOpcoes(hist);
                    ViewBag.IdSessao = historia.chat_id;
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
        public async Task<IActionResult> PostSpeech(string historiaTexto)
        {
            // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"

            try
            {
                var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
                speechConfig.SpeechSynthesisVoiceName = "pt-BR-ElzaNeural";

                //check if there is audio playing, and if there is, stop it
                await fala.StopSpeakingAsync();

                var speechSynthesisResult = await fala.SpeakTextAsync(historiaTexto);

                // OutputSpeechSynthesisResult(speechSynthesisResult, text); // Pode não ser necessário para o retorno HTTP

                // Verifica o resultado da síntese de fala
                if (speechSynthesisResult.Reason == ResultReason.SynthesizingAudioCompleted)
                {
                    // Se a síntese foi completada com sucesso, retornar um OkResult
                    return Ok(new { Status = "Completed", Text = historiaTexto });
                }
                else
                {
                    // Se houve cancelamento ou erro na síntese
                    var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                    return BadRequest($"Speech synthesis failed. Reason={cancellation.Reason}, ErrorCode={cancellation.ErrorCode}, ErrorDetails={cancellation.ErrorDetails}");
                }
            }
            catch (Exception ex)
            {
                // Captura de exceções gerais
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        private static List<string> FormataOpcoes(string historia)
        {
            string opcoes = historia.Split("---")[1];
            string[] opcao = opcoes.Split("|");
            List<string> opcoes_formatadas = [];
            for (int i = 0; i < opcao.Length; i++)
            {
                //pula as strings inuteis para o sistema
                if (i == 1 || i == 3 || i == 5)
                    opcoes_formatadas.Add(opcao[i]);
            }
            return opcoes_formatadas;
        }

        public static string FormataHistoria(string historia)
        {
            return historia.Split("---")[0];
        }
    }
}
