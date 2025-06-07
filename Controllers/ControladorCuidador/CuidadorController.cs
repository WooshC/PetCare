using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using PetCare.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetCare.Controllers
{
    [Route("Cuidador")]
    public class CuidadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CuidadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Cuidador")]
        public async Task<IActionResult> Cuidador(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var cuidador = await _context.Cuidadores
                .Include(c => c.Usuario)
                .Include(c => c.Calificaciones)
                .ThenInclude(cal => cal.Cliente) // Si tienes relación con Cliente
                .AsNoTracking() // Mejora rendimiento para operaciones de solo lectura
                .FirstOrDefaultAsync(c => c.UsuarioID == id.Value);

            if (cuidador == null)
            {
                return NotFound();
            }

            // Actualiza el promedio llamando al método del modelo
            cuidador.ActualizarPromedio();

            return View(cuidador);
        }

    }
}