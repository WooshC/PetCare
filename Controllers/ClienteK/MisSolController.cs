using Microsoft.AspNetCore.Mvc;

namespace PetCare.Controllers.ClienteK
{
    public class MisSolController:Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
           
            return View("~/Views/ClienteK/MisSol/Index.cshtml");
        }
    }
}
