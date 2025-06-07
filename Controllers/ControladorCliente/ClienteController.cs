using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;
using PetCare.Services;

namespace PetCare.Controllers.ControladorCliente
{
    public class ClienteController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ClienteStrategy _clienteStrategy;

        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
            _clienteStrategy = new ClienteStrategy();
        }

        // Vista principal del cliente
        public async Task<IActionResult> Cliente()
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioID == 1); // Simulado
            return await _clienteStrategy.HandleRequestAsync(_context, this, usuario);
        }

        [HttpPost]
        public async Task<IActionResult> CrearSolicitud(Solicitud solicitud)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Formulario inválido.";
                return RedirectToAction("Cliente");
            }

            solicitud.FechaCreacion = DateTime.Now;
            solicitud.Estado = "Pendiente";
            solicitud.ClienteID = 1; // Simulado - reemplazar por ID real del cliente

            // Temporalmente asignar a un cuidador por defecto (debería seleccionarse después)
            solicitud.CuidadorID = _context.Cuidadores.Select(c => c.CuidadorID).FirstOrDefault();

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "Solicitud creada exitosamente.";
            return RedirectToAction("Cliente");
        }

        [HttpPost]
        public async Task<IActionResult> Calificar(int SolicitudID, Calificacion calificacion)
        {
            var solicitud = await _context.Solicitudes
                .Include(s => s.Cuidador)
                .FirstOrDefaultAsync(s => s.SolicitudID == SolicitudID);

            if (solicitud == null)
            {
                TempData["Error"] = "Solicitud no encontrada.";
                return RedirectToAction("Cliente");
            }

            calificacion.CuidadorID = solicitud.CuidadorID;
            calificacion.ClienteID = solicitud.ClienteID;
            calificacion.FechaCalificacion = DateTime.Now;

            _context.Calificaciones.Add(calificacion);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "Servicio calificado correctamente.";
            return RedirectToAction("Cliente");
        }

    }
}
