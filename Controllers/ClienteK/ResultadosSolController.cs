using Microsoft.AspNetCore.Mvc;

namespace PetCare.Controllers.ClienteK
{
    public class ResultadosSolController : Controller
    {
        [HttpGet]
        public IActionResult Index(int id)
        {
            ViewData["SolicitudId"] = id;
            return View("~/Views/ClienteK/ResultadosSol/Index.cshtml");
        }

    }
}
