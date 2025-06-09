using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using PetCare.Services;
using System.Threading.Tasks;

namespace PetCare.Controllers
{
    public class RedirectController : BaseController
    {
        public RedirectController(ApplicationDbContext context, RoleStrategyFactory roleStrategyFactory)
            : base(context, roleStrategyFactory)
        {
        }

        [HttpGet]
        [Route("Redirect/ToDashboard")]
        public async Task<IActionResult> ToDashboard()
        {
            return await RedirectByRole();
        }
    }
}