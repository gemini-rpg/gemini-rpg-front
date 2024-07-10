namespace AiRpgFrontEnd.Models
{
    public class HistoricoViewModel
    {
        public Guid chat_id { get; set; }
        public List<MensagemViewModel> message { get; set; }
    }
}
