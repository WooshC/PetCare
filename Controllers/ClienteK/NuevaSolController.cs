using Microsoft.AspNetCore.Mvc;

namespace PetCare.Controllers.ClienteK
{
    public class NuevaSolController:Controller 
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/ClienteK/NuevaSol/Index.cshtml");
        }

    }
}
