// PetCare/Services/CuidadorStrategy.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;

namespace PetCare.Services
{
    public class CuidadorStrategy : IRoleStrategy
    {
        public async Task<IActionResult> HandleRequestAsync(ApplicationDbContext context, Controller controller, Usuario usuario)
        {
            var cuidador = await context.Cuidadores
                .Include(c => c.Usuario)
                .Include(c => c.Calificaciones) // Usa el nombre correcto de la propiedad
                .FirstOrDefaultAsync(c => c.UsuarioID == usuario.UsuarioID);

            if (cuidador == null)
            {
                return controller.NotFound();
            }

            if (cuidador.Calificaciones.Any()) // Actualizado para usar la propiedad correcta
            {
                cuidador.CalificacionPromedio = (decimal)cuidador.Calificaciones
                    .Average(c => c.Puntuacion);
            }

            return controller.View("~/Views/Cuidador/Cuidador.cshtml", cuidador);
        }

        public Task<IActionResult> HandleRequestAsync(Controller controller, Usuario usuario)
        {
            throw new NotImplementedException();
        }
    }
}