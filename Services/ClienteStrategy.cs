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
            var cliente = await context.Clientes
                .Include(c => c.Usuario)
                .Include(c => c.Solicitudes)
                    .ThenInclude(s => s.Cuidador)
                        .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.UsuarioID == usuario.UsuarioID);

            if (cliente == null)
            {
                return controller.NotFound();
            }

            // Obtener todos los cuidadores con su info de usuario
            var todosLosCuidadores = await context.Cuidadores
                .Include(c => c.Usuario)
                .ToListAsync();

            // Para cada solicitud activa, asignar cuidadores sugeridos
            foreach (var solicitud in cliente.Solicitudes
                .Where(s => s.Estado == "Pendiente" || s.Estado == "Aceptada"))
            {
                var sugeridos = todosLosCuidadores
                    .Where(c => !context.Solicitudes.Any(s =>
                        s.CuidadorID == c.CuidadorID &&
                        s.ClienteID == cliente.ClienteID &&
                        s.Estado == "Aceptada"))
                    .ToList();

                solicitud.CuidadoresRelacionados = sugeridos;
            }

            return controller.View("~/Views/Cliente/Cliente.cshtml", cliente);
        }


        public Task<IActionResult> HandleRequestAsync(Controller controller, Usuario usuario)
        {
            throw new NotImplementedException();
        }
    }
}
