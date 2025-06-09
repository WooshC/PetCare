using PetCare.Data;
using PetCare.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetCare.Services
{
    public class SolicitudClienteService : ISolicitudClienteService
    {
        private readonly ApplicationDbContext _context;

        public SolicitudClienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Solicitud>> GetSolicitudesPendientes(int clienteId)
        {
            return await _context.Solicitudes
                .Include(s => s.Cuidador)
                    .ThenInclude(c => c.Usuario)
                .Where(s => s.ClienteID == clienteId && s.Estado == "Pendiente")
                .OrderBy(s => s.FechaHoraInicio)
                .ToListAsync();
        }

        public async Task<IEnumerable<Solicitud>> GetHistorialServicios(int clienteId)
        {
            return await _context.Solicitudes
                .Include(s => s.Cuidador)
                    .ThenInclude(c => c.Usuario)
                .Where(s => s.ClienteID == clienteId && (s.Estado == "Finalizada" || s.Estado == "Rechazada"))
                .OrderByDescending(s => s.FechaHoraInicio)
                .Take(10) // Limitar a los últimos 10 servicios
                .ToListAsync();
        }

        public async Task<bool> CancelarSolicitud(int solicitudId)
        {
            var solicitud = await _context.Solicitudes.FindAsync(solicitudId);
            if (solicitud == null || solicitud.Estado != "Pendiente")
            {
                return false;
            }

            solicitud.Estado = "Rechazada";
            solicitud.FechaActualizacion = DateTime.Now;

            _context.Solicitudes.Update(solicitud);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Solicitud>> GetSolicitudesActivas(int clienteId)
        {
            return await _context.Solicitudes
                .Where(s => s.ClienteID == clienteId &&
                      (s.Estado == "Aceptada" || s.Estado == "En Progreso"))
                .Include(s => s.Cuidador)
                    .ThenInclude(c => c.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cuidador>> GetCuidadoresDisponibles()
        {
            var ahora = DateTime.Now;

            // Obtener cuidadores que no tienen solicitudes activas
            var cuidadoresOcupadosIds = await _context.Solicitudes
                .Where(s => s.Estado == "Aceptada" ||
                             s.Estado == "En Progreso" ||
                             s.Estado == "Fuera de Tiempo")
                .Select(s => s.CuidadorID)
                .Distinct()
                .ToListAsync();

            return await _context.Cuidadores
                .Include(c => c.Usuario)
                .Where(c => c.DocumentoVerificado &&
                            c.Disponible &&
                            !cuidadoresOcupadosIds.Contains(c.CuidadorID))
                .OrderByDescending(c => c.CalificacionPromedio)
                .ToListAsync();
        }
    }
}