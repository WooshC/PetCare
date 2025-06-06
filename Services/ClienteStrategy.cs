using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;

namespace PetCare.Services
{
    public class ClienteStrategy : IRoleStrategy
    {
        public async Task<IActionResult> HandleRequestAsync(ApplicationDbContext context, Controller controller, Usuario usuario)
        {
            // Lógica específica para clientes
            var cliente = await context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.UsuarioID == usuario.UsuarioID);

            return controller.View("~/Views/Cliente/Cliente.cshtml", cliente);
        }

        public Task<IActionResult> HandleRequestAsync(Controller controller, Usuario usuario)
        {
            throw new NotImplementedException();
        }
    }
}
