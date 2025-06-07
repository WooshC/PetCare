// Services/SolicitudService.cs
using PetCare.Data;
using PetCare.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetCare.Models.Solicitudes;

namespace PetCare.Services
{
    public class SolicitudService : ISolicitudService
    {
        private readonly ApplicationDbContext _context;

        public SolicitudService(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<IEnumerable<Solicitud>> GetHistorialServicios(int cuidadorId)
        {
            return await _context.Solicitudes
                .Include(s => s.Cliente)
                    .ThenInclude(c => c.Usuario)
                .Where(s => s.CuidadorID == cuidadorId && (s.Estado == "Finalizada" || s.Estado == "Rechazada"))
                .OrderByDescending(s => s.FechaHoraInicio)
                .Take(10) // Limitar a los últimos 10 servicios
                .ToListAsync();
        }

        public async Task<bool> CambiarEstadoSolicitud(int solicitudId, string nuevoEstado)
        {
            var solicitud = await _context.Solicitudes.FindAsync(solicitudId);
            if (solicitud == null)
            {
                return false;
            }

            solicitud.Estado = nuevoEstado;
            solicitud.FechaActualizacion = DateTime.Now;

            _context.Solicitudes.Update(solicitud);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}