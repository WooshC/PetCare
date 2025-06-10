using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using PetCare.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using PetCare.Models.ViewModels;
using PetCare.Services.Solicitudes;

namespace PetCare.Controllers
{
    [Authorize(Roles = "Cliente")]
    [Route("Cliente")]
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISolicitudClienteService _solicitudClienteService;

        public ClienteController(
       ApplicationDbContext context,
       ISolicitudClienteService solicitudClienteService)
        {
            _context = context;
            _solicitudClienteService = solicitudClienteService;
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


        [HttpGet("PerfilCuidador/{id}")]
        public async Task<IActionResult> PerfilCuidador(int id)
        {
            var cuidador = await _context.Cuidadores
                .Include(c => c.Usuario)
                .Include(c => c.Calificaciones)
                    .ThenInclude(cal => cal.Cliente)
                        .ThenInclude(cli => cli.Usuario)
                .FirstOrDefaultAsync(c => c.CuidadorID == id);

            if (cuidador == null)
            {
                return NotFound();
            }

            return View("PerfilCuidador", cuidador);
        }


        [HttpGet("SolicitarServicio/{cuidadorId}")]
        public IActionResult SolicitarServicio(int cuidadorId)
        {
            var model = new SolicitudServicioViewModel
            {
                CuidadorID = cuidadorId,
                FechaHoraInicio = DateTime.Now.AddDays(1)
            };

            // Si es AJAX, retorna vista parcial
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_SolicitudServicioPartial", model);
            }

            // Fallback normal
            return View(model);
        }


        [HttpPost("SolicitarServicio")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarServicio(SolicitudServicioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var clienteId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _solicitudClienteService.CrearSolicitud(model, clienteId);

            if (!success)
            {
                ModelState.AddModelError("", "Ocurrió un error al crear la solicitud");
                return View(model);
            }

            TempData["SuccessMessage"] = "Solicitud enviada correctamente";
            return RedirectToAction("Dashboard");
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

        // Método privado que encapsula la obtención del ViewModel para el dashboard del cliente.
        private async Task<ClienteDashboardViewModel> ObtenerViewModel(int usuarioId)
        {
            var cliente = await ObtenerCliente(usuarioId);
            if (cliente == null)
            {
                return null;
            }

            var cuidadoresDisponibles = await ObtenerCuidadoresDisponibles();

            return new ClienteDashboardViewModel
            {
                Cliente = cliente,
                SolicitudesPendientes = await _solicitudClienteService.GetSolicitudesPendientes(cliente.ClienteID),
                SolicitudesActivas = await _solicitudClienteService.GetSolicitudesActivas(cliente.ClienteID),
                HistorialServicios = await _solicitudClienteService.GetHistorialServicios(cliente.ClienteID),
                CuidadoresDisponibles = cuidadoresDisponibles ?? new List<Cuidador>()
            };
        }

        // Método privado que encapsula la obtención del cliente (incluyendo sus solicitudes y datos de usuario).
        private async Task<Cliente> ObtenerCliente(int usuarioId)
        {
            return await _context.Clientes
                .Include(c => c.Usuario)
                .Include(c => c.Solicitudes)
                    .ThenInclude(s => s.Cuidador)
                        .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.UsuarioID == usuarioId);
        }

        // Método privado que encapsula la obtención de cuidadores disponibles.
        private async Task<IEnumerable<Cuidador>> ObtenerCuidadoresDisponibles()
        {
            return await _solicitudClienteService.GetCuidadoresDisponibles();
        }
    }
}