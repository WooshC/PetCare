using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PetCare.Models;
using PetCare.Data;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PetCare.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> VerificacionUsuarios()
        {
            var usuariosPendientes = await _context.Usuarios
                .Include(u => u.DocumentosVerificacion)
                .Where(u => u.DocumentosVerificacion.Any(d => d.Estado == "Pendiente"))
                .ToListAsync();

            return View(usuariosPendientes);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerDocumento(int documentoId)
        {
            var documento = await _context.DocumentosVerificacion
                .FirstOrDefaultAsync(d => d.DocumentoID == documentoId);
                
            if (documento == null)
                return NotFound();

            return File(documento.Archivo, "application/octet-stream");
        }

        [HttpPost]
        public async Task<IActionResult> VerificarDocumento(int documentoId, bool aprobado, string observaciones)
        {
            var documento = await _context.DocumentosVerificacion
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.DocumentoID == documentoId);

            if (documento == null)
                return NotFound();

            documento.Estado = aprobado ? "Aprobado" : "Rechazado";
            documento.Observaciones = observaciones;
            documento.FechaVerificacion = DateTime.Now;

            // Actualizar estado del usuario si todos sus documentos están aprobados
            var usuario = documento.Usuario;
            if (usuario != null)
            {
                var todosDocumentosAprobados = await _context.DocumentosVerificacion
                    .Where(d => d.UsuarioID == usuario.UsuarioID)
                    .AllAsync(d => d.Estado == "Aprobado");

                if (todosDocumentosAprobados)
                {
                    usuario.Verificado = true;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(VerificacionUsuarios));
        }

        public async Task<IActionResult> Reservas()
        {
            var solicitudes = await _context.Solicitudes
                .Include(s => s.Cliente)
                    .ThenInclude(c => c.Usuario)
                .Include(s => s.Cuidador)
                    .ThenInclude(c => c.Usuario)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();

            return View(solicitudes);
        }

        public async Task<IActionResult> VerDetalles(int id)
        {
            var solicitud = await _context.Solicitudes
                .Include(s => s.Cliente)
                    .ThenInclude(c => c.Usuario)
                .Include(s => s.Cuidador)
                    .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(s => s.SolicitudID == id);

            if (solicitud == null)
            {
                return NotFound();
            }

            return View(solicitud);
        }

        public async Task<IActionResult> Valoraciones()
        {
            var calificaciones = await _context.Calificaciones
                .Include(c => c.Cuidador)
                    .ThenInclude(c => c.Usuario)
                .Include(c => c.Cliente)
                    .ThenInclude(c => c.Usuario)
                .OrderByDescending(c => c.FechaCalificacion)
                .ToListAsync();

            return View(calificaciones);
        }
    }
} 