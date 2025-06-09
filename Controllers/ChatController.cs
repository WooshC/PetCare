using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using PetCare.Services;
using PetCare.Data;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PetCare.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ApplicationDbContext _context;

        public ChatController(IHubContext<ChatHub> hubContext, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }

        public async Task<IActionResult> Index(int solicitudId)
        {
            // Verificar que la solicitud existe y el usuario tiene permiso
            var solicitud = await _context.Solicitudes
                .Include(s => s.Cliente)
                .Include(s => s.Cuidador)
                .FirstOrDefaultAsync(s => s.SolicitudID == solicitudId);

            if (solicitud == null)
                return NotFound();

            // Verificar que el usuario es parte de la solicitud
            var usuarioId = int.Parse(User.FindFirst("UsuarioID")?.Value ?? "0");
            if (solicitud.ClienteID != usuarioId && solicitud.CuidadorID != usuarioId)
                return Forbid();

            ViewBag.Solicitud = solicitud;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EnviarMensaje(int solicitudId, string mensaje)
        {
            // Verificar que la solicitud existe
            var solicitud = await _context.Solicitudes
                .FirstOrDefaultAsync(s => s.SolicitudID == solicitudId);

            if (solicitud == null)
                return NotFound();

            // Enviar mensaje a través de SignalR
            await _hubContext.Clients.Group(solicitudId.ToString())
                .SendAsync("ReceiveMessage", User.Identity.Name, mensaje);

            return Ok();
        }
    }
} 