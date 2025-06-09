using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;
using PetCare.Models.ViewModels;
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
            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioID == usuarioId);
            return await _clienteStrategy.HandleRequestAsync(_context, this, usuario);
        }

        [HttpPost]
        public async Task<IActionResult> CrearSolicitud(NuevaSolicitudViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Formulario inválido.";
                return RedirectToAction("Cliente");
            }

            var usuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioID == usuarioId);

            if (cliente == null)
            {
                TempData["Error"] = "Cliente no encontrado.";
                return RedirectToAction("Cliente");
            }

            var solicitud = new Solicitud
            {
                ClienteID = cliente.ClienteID,
                TipoServicio = modelo.TipoServicio,
                Ubicacion = modelo.Ubicacion,
                DuracionHoras = modelo.DuracionHoras,
                Descripcion = modelo.Descripcion,
                Estado = "Pendiente",
                FechaCreacion = DateTime.Now,
                FechaHoraInicio = DateTime.Now,
                HoraDeseada = modelo.HoraDeseada

            };

            _context.Solicitudes.Add(solicitud);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "Solicitud creada exitosamente.";
            return RedirectToAction("Cliente");
        }

        [HttpPost]
        public async Task<IActionResult> AsignarCuidador(int solicitudId, int cuidadorId)
        {
            // Creamos una solicitud "fantasma" solo con el ID
            var solicitud = new Solicitud { SolicitudID = solicitudId };

            // Adjuntamos la entidad al contexto para que EF pueda rastrearla
            _context.Solicitudes.Attach(solicitud);

            // Asignamos el nuevo cuidador y la fecha
            solicitud.CuidadorID = cuidadorId;
            solicitud.FechaActualizacion = DateTime.Now;

            // Indicamos explícitamente que esas propiedades cambiaron
            _context.Entry(solicitud).Property(s => s.CuidadorID).IsModified = true;
            _context.Entry(solicitud).Property(s => s.FechaActualizacion).IsModified = true;

            // Guardamos los cambios
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "Has solicitado a un cuidador. Espera su confirmación.";
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

            if (solicitud.CuidadorID == null)
            {
                TempData["Error"] = "Esta solicitud aún no tiene un cuidador asignado. No se puede calificar.";
                return RedirectToAction("Cliente");
            }

            calificacion.CuidadorID = solicitud.CuidadorID.Value;
            calificacion.ClienteID = solicitud.ClienteID;
            calificacion.FechaCalificacion = DateTime.Now;

            _context.Calificaciones.Add(calificacion);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] = "Servicio calificado correctamente.";
            return RedirectToAction("Cliente");
        }

    }
}
