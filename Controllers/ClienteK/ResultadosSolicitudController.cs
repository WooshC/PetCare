using Microsoft.AspNetCore.Mvc;

namespace PetCare.Controllers.ClienteK
{
    public class ResultadosSolicitudController : Controller
    {
        // GET: /Cliente/ResultadosSolicitud
        public IActionResult Index()
        {
            // Por ahora no pasamos ningún modelo; la vista mostrará contenido estático o
            // placeholders. Luego conectaremos la capa de datos.
            return View("~/Views/ClienteK/ResultadosSolicitud/Index.cshtml");
        }

        // GET: /Cliente/ResultadosSolicitud/Detalle/5
        public IActionResult Detalle(int id)
        {
            // El parámetro "id" será el ID del cuidador. Por ahora, puedes ignorarlo
            // y mostrar un mensaje genérico. Luego aquí cargarás los datos reales.
            ViewBag.CuidadorId = id;
            return View("~/Views/ClienteK/ResultadosSolicitud/Detalle.cshtml");
        }
    }
}
