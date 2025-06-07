using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using PetCare.Services;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PetCare.Controllers
{
    [Authorize(Roles = "Cuidador")] // Solo usuarios con rol Cuidador pueden acceder
    [Route("Cuidador")]
    public class CuidadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CuidadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Cuidador")]
        public async Task<IActionResult> Cuidador()
        {
            // Obtener el ID del usuario autenticado
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Verificar si el usuario es realmente un cuidador
            var esCuidador = await _context.UsuarioRoles
                .AnyAsync(ur => ur.UsuarioID == userId && ur.Rol.NombreRol == "Cuidador");

            if (!esCuidador)
            {
                return Forbid(); // O redirigir a acceso denegado
            }

            var cuidador = await _context.Cuidadores
                .Include(c => c.Usuario)
                .Include(c => c.Calificaciones)
                .ThenInclude(cal => cal.Cliente)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UsuarioID == userId);

            if (cuidador == null)
            {
                return NotFound();
            }

            cuidador.ActualizarPromedio();
            return View(cuidador);
        }
    }
}