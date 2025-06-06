using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using PetCare.Models;

namespace PetCare.Services
{
    public class AdminStrategy : IRoleStrategy
    {
        public async Task<IActionResult> HandleRequestAsync(ApplicationDbContext context, Controller controller, Usuario usuario)
        {
            // Lógica específica para administradores
            return await Task.FromResult(controller.RedirectToAction("Index", "Admin"));
        }

        public Task<IActionResult> HandleRequestAsync(Controller controller, Usuario usuario)
        {
            throw new NotImplementedException();
        }
    }
}