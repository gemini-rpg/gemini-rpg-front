using System.Xml.Linq;

namespace AiRpgFrontEnd.Models
{
    public class HistoriaViewModel
    {
        public HistoriaViewModel(Guid id, string message)
        {
            chat_id = id;
            this.message = message;
        }
        public Guid chat_id { get; set; }

        public string message { get; set; }
    }
}
