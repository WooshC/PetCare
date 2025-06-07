using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using PetCare.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PetCare.Models.ViewModels;
using PetCare.Services;

namespace PetCare.Controllers
{
    [Authorize(Roles = "Cuidador")]
    [Route("Cuidador")]
    public class CuidadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISolicitudService _solicitudService;

        public CuidadorController(ApplicationDbContext context, ISolicitudService solicitudService)
        {
            _context = context;
            _solicitudService = solicitudService;
        }

        [HttpGet("Dashboard")]
        [HttpGet("")] // Maneja tanto /Cuidador como /Cuidador/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var viewModel = await ObtenerViewModel(userId);
            return View("Cuidador", viewModel); // Especifica el nombre de la vista
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Perfil(int id)
        {
            var viewModel = await ObtenerViewModel(id);
            return View("Cuidador", viewModel); // Usa la misma vista
        }

        [HttpPost("AceptarSolicitud/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceptarSolicitud(int id)
        {
            var result = await _solicitudService.CambiarEstadoSolicitud(id, "Aceptada");
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction("Dashboard");
        }

        [HttpPost("RechazarSolicitud/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarSolicitud(int id)
        {
            var result = await _solicitudService.CambiarEstadoSolicitud(id, "Rechazada");
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction("Dashboard");
        }

        private async Task<CuidadorDashboardViewModel> ObtenerViewModel(int usuarioId)
        {
            var cuidador = await _context.Cuidadores
                .Include(c => c.Usuario)
                .Include(c => c.Calificaciones)
                    .ThenInclude(cal => cal.Cliente)
                        .ThenInclude(cli => cli.Usuario)
                .FirstOrDefaultAsync(c => c.UsuarioID == usuarioId);

            if (cuidador == null)
            {
                return null;
            }

            cuidador.ActualizarPromedio();

            return new CuidadorDashboardViewModel
            {
                Cuidador = cuidador,
                SolicitudesPendientes = await _solicitudService.GetSolicitudesPendientes(cuidador.CuidadorID),
                HistorialServicios = await _solicitudService.GetHistorialServicios(cuidador.CuidadorID)
            };
        }
    }
}