using Microsoft.AspNetCore.Mvc;
using PetCare.Models;

namespace PetCare.Services
{
    public interface IRoleStrategy
    {
        Task<IActionResult> HandleRequestAsync(Data.ApplicationDbContext context, Controller controller, Usuario usuario);
    }
}
