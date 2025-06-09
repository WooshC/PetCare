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
    [Authorize(Roles = "Cliente")]
    [Route("Cliente")]
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISolicitudService _solicitudService;

        public ClienteController(ApplicationDbContext context, ISolicitudService solicitudService)
        {
            _context = context;
            _solicitudService = solicitudService;
        }

        [HttpGet("Dashboard")]
        [HttpGet("")] // Maneja tanto /Cliente como /Cliente/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var viewModel = await ObtenerViewModel(userId);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View("Cliente", viewModel); // Especifica el nombre de la vista
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Perfil(int id)
        {
            var viewModel = await ObtenerViewModel(id);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View("Cliente", viewModel); // Usa la misma vista
        }

        private async Task<ClienteDashboardViewModel> ObtenerViewModel(int usuarioId)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .Include(c => c.Solicitudes)
                    .ThenInclude(s => s.Cuidador)
                        .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.UsuarioID == usuarioId);

            if (cliente == null)
            {
                return null;
            }

            return new ClienteDashboardViewModel
            {
                Cliente = cliente,
                // Inicializamos las demás propiedades como listas vacías por ahora
                SolicitudesPendientes = new List<Solicitud>(),
                SolicitudesActivas = new List<Solicitud>(),
                HistorialServicios = new List<Solicitud>(),
            };
        }
    }
}