using Microsoft.AspNetCore.Mvc;
namespace PetCare.Controllers.ControladorCuidador
{
    public class CuidadorController : Controller
    {
        public IActionResult Cuidador()
        {
            return View(); // Esto buscará Views/Cuidador/Cuidador.cshtml
        }

        public IActionResult DetalleSolicitud()
        {
            return View(); // Esto buscará Views/Cuidador/DetalleSolicitud.cshtml
        }
    }
}