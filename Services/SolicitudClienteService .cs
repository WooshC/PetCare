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
        private readonly ICuidadorService _cuidadorService;

        public SolicitudClienteService(ApplicationDbContext context, ICuidadorService cuidadorService)
        {
            _context = context;
            _cuidadorService = cuidadorService;
        }

        public async Task<IEnumerable<Cuidador>> GetCuidadoresDisponibles()
        {
            if (_cuidadorService == null)
            {
                throw new InvalidOperationException("CuidadorService no está inicializado");
            }
            return await _cuidadorService.GetCuidadoresDisponibles();
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
        public async Task<bool> CrearSolicitud(SolicitudServicioViewModel model, int clienteId)
        {
            try
            {
                var solicitud = new Solicitud
                {
                    ClienteID = clienteId,
                    CuidadorID = model.CuidadorID,
                    TipoServicio = model.TipoServicio,
                    Descripcion = model.Descripcion,
                    FechaHoraInicio = model.FechaHoraInicio,
                    DuracionHoras = model.DuracionHoras,
                    Ubicacion = model.Ubicacion,
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                };

                _context.Solicitudes.Add(solicitud);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /*public async Task<IEnumerable<Cuidador>> GetCuidadoresDisponibles()
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
        */

    }
}