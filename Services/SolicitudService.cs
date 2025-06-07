// Services/SolicitudService.cs
using PetCare.Data;
using PetCare.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetCare.Models.ViewModels;

namespace PetCare.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ApplicationDbContext _context;

        public SolicitudService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarEstadosAutomaticos()
        {
            var ahora = DateTime.Now;

            // Actualizar a "En Progreso" las aceptadas que hayan llegado a su hora
            var solicitudesParaIniciar = await _context.Solicitudes
                .Where(s => s.Estado == "Aceptada" && s.FechaHoraInicio <= ahora)
                .ToListAsync();

            foreach (var solicitud in solicitudesParaIniciar)
            {
                solicitud.Estado = "En Progreso";
                solicitud.FechaInicioServicio = ahora;
                solicitud.FechaActualizacion = ahora;
            }

            // Actualizar a "Fuera de Tiempo" las que hayan excedido su duración
            var solicitudesFueraDeTiempo = await _context.Solicitudes
                .Where(s => s.Estado == "En Progreso" &&
                       s.FechaHoraInicio.AddHours(s.DuracionHoras) < ahora)
                .ToListAsync();

            foreach (var solicitud in solicitudesFueraDeTiempo)
            {
                solicitud.Estado = "Fuera de Tiempo";
                solicitud.FechaActualizacion = ahora;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CambiarEstadoSolicitud(int solicitudId, string nuevoEstado)
        {
            var solicitud = await _context.Solicitudes.FindAsync(solicitudId);
            if (solicitud == null) return false;

            // Validar transiciones de estado
            if (!ValidarTransicionEstado(solicitud.Estado, nuevoEstado))
            {
                return false;
            }

            solicitud.Estado = nuevoEstado;
            solicitud.FechaActualizacion = DateTime.Now;

            // Registrar fechas importantes
            switch (nuevoEstado)
            {
                case "Aceptada":
                    solicitud.FechaAceptacion = DateTime.Now;
                    break;
                case "Finalizada":
                    solicitud.FechaFinalizacion = DateTime.Now;
                    break;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        private bool ValidarTransicionEstado(string estadoActual, string nuevoEstado)
        {
            var transicionesValidas = new Dictionary<string, List<string>>
            {
                ["Pendiente"] = new List<string> { "Aceptada", "Rechazada" },
                ["Aceptada"] = new List<string> { "En Progreso" }, // Automático
                ["En Progreso"] = new List<string> { "Finalizada", "Fuera de Tiempo" },
                ["Fuera de Tiempo"] = new List<string> { "Finalizada" }
            };

            return transicionesValidas.ContainsKey(estadoActual) &&
                   transicionesValidas[estadoActual].Contains(nuevoEstado);
        }

        public async Task<bool> FinalizarSolicitud(int solicitudId, int clienteId)
        {
            var solicitud = await _context.Solicitudes
                .FirstOrDefaultAsync(s => s.SolicitudID == solicitudId && s.ClienteID == clienteId);

            if (solicitud == null) return false;

            if (solicitud.Estado != "En Progreso" && solicitud.Estado != "Fuera de Tiempo")
            {
                return false;
            }

            return await CambiarEstadoSolicitud(solicitudId, "Finalizada");
        }

        public async Task<IEnumerable<Solicitud>> GetSolicitudesPendientes(int cuidadorId)
        {
            return await _context.Solicitudes
                .Include(s => s.Cliente)
                    .ThenInclude(c => c.Usuario)
                .Where(s => s.CuidadorID == cuidadorId && s.Estado == "Pendiente")
                .OrderBy(s => s.FechaHoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Solicitud>> GetSolicitudesActivas(int cuidadorId)
        {
            return await _context.Solicitudes
                .Include(s => s.Cliente)
                    .ThenInclude(c => c.Usuario)
                .Where(s => s.CuidadorID == cuidadorId &&
                      (s.Estado == "Aceptada" || s.Estado == "En Progreso"))
                .OrderBy(s => s.FechaHoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Solicitud>> GetHistorialServicios(int cuidadorId)
        {
            return await _context.Solicitudes
                .Include(s => s.Cliente)
                    .ThenInclude(c => c.Usuario)
                .Where(s => s.CuidadorID == cuidadorId &&
                      (s.Estado == "Finalizada" || s.Estado == "Rechazada" || s.Estado == "Fuera de Tiempo"))
                .OrderByDescending(s => s.FechaHoraInicio)
                .Take(10)
                .ToListAsync();
        }
    }
}