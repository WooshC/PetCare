using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;

namespace PetCare.Services.AuthServices
{
    public class CuidadorStrategy : IRoleStrategy
    {
        private readonly ApplicationDbContext _context;

        public CuidadorStrategy(ApplicationDbContext context)
        {
            _context = context;
        }

        // Implementación completa del método
        public async Task<IActionResult> HandleRequestAsync(ApplicationDbContext context, Controller controller, Usuario usuario)
        {
            var cuidador = await _context.Cuidadores
                .Include(c => c.Usuario)
                .Include(c => c.Calificaciones)
                .FirstOrDefaultAsync(c => c.UsuarioID == usuario.UsuarioID);

            if (cuidador == null)
            {
                return controller.NotFound();
            }

            if (cuidador.Calificaciones.Any())
            {
                cuidador.CalificacionPromedio = (decimal)cuidador.Calificaciones
                    .Average(c => c.Puntuacion);
            }

            // Redirige al Dashboard que sí existe
            return controller.RedirectToAction("Dashboard", "Cuidador");
        }

        // Implementación alternativa si es necesaria
        public async Task<IActionResult> HandleRequestAsync(Controller controller, Usuario usuario)
        {
            return await HandleRequestAsync(_context, controller, usuario);
        }
    }
}