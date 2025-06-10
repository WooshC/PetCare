// Services/SolicitudService.cs
using PetCare.Data;
using PetCare.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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
            return await _context.GetSolicitudesByEstado(cuidadorId, new[] { "Pendiente" });
        }

        public async Task<IEnumerable<Solicitud>> GetHistorialServicios(int cuidadorId)
        {
            return await _context.GetSolicitudesByEstado(
                cuidadorId,
                new[] { "Finalizada", "Rechazada" },
                take: 10);
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

        public async Task<IEnumerable<Solicitud>> GetSolicitudesActivas(int cuidadorId)
        {
            return await _context.GetSolicitudesByEstado(
                cuidadorId,
                new[] { "Aceptada", "En Progreso" });
        }

        public async Task<IEnumerable<Solicitud>> GetSolicitudesFueraDeTiempo(int cuidadorId)
        {
            var ahora = DateTime.Now;
            var solicitudes = await _context.Solicitudes
                .IncludeClienteUsuario()
                .Where(s => s.CuidadorID == cuidadorId &&
                       (s.Estado == "Fuera de Tiempo" ||
                        (s.Estado == "En Progreso" &&
                         s.FechaHoraInicio.AddHours(s.DuracionHoras) < ahora)))
                .OrderByDescending(s => s.FechaHoraInicio)
                .ToListAsync();

            // Actualizar estado de solicitudes fuera de tiempo
            foreach (var solicitud in solicitudes.Where(s => s.Estado == "En Progreso"))
            {
                solicitud.Estado = "Fuera de Tiempo";
                solicitud.FechaActualizacion = ahora;
            }

            if (solicitudes.Any(s => s.Estado == "En Progreso"))
            {
                await _context.SaveChangesAsync();
            }

            return solicitudes;
        }
    }
}