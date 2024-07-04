using Microsoft.AspNetCore.Mvc;

namespace AiRpgFrontEnd.ViewComponents
{
    public class HistoriaViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
