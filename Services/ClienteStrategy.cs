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
                if (solicitud.HoraDeseada == null)
                    continue;

                var inicioDeseado = solicitud.FechaHoraInicio.Date + solicitud.HoraDeseada.Value;
                var finDeseado = inicioDeseado.AddHours(solicitud.DuracionHoras);

                // Traer a memoria todas las solicitudes aceptadas con HoraDeseada
                var solicitudesAceptadas = await context.Solicitudes
                    .Where(s => s.Estado == "Aceptada" && s.HoraDeseada != null)
                    .ToListAsync();

                var sugeridos = todosLosCuidadores
                    .Where(c =>
                        !solicitudesAceptadas.Any(s =>
                            s.CuidadorID == c.CuidadorID &&
                            (s.FechaHoraInicio.Date + s.HoraDeseada.Value) < finDeseado &&
                            (s.FechaHoraInicio.Date + s.HoraDeseada.Value).AddHours(s.DuracionHoras) > inicioDeseado
                        )
                    ).ToList();

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
