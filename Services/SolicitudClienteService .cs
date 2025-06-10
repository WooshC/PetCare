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
            return await GetSolicitudesPendientesQuery(clienteId).ToListAsync();
        }

        private IQueryable<Solicitud> GetSolicitudesPendientesQuery(int clienteId)
        {
            return _context.Solicitudes
                .Include(s => s.Cuidador)
                    .ThenInclude(c => c.Usuario)
                .Where(s => s.ClienteID == clienteId && s.Estado == "Pendiente")
                .OrderBy(s => s.FechaHoraInicio);
        }

        public async Task<IEnumerable<Solicitud>> GetHistorialServicios(int clienteId)
        {
            return await GetHistorialServiciosQuery(clienteId).ToListAsync();
        }

        private IQueryable<Solicitud> GetHistorialServiciosQuery(int clienteId)
        {
            return _context.Solicitudes
                .Include(s => s.Cuidador)
                    .ThenInclude(c => c.Usuario)
                .Where(s => s.ClienteID == clienteId && (s.Estado == "Finalizada" || s.Estado == "Rechazada"))
                .OrderByDescending(s => s.FechaHoraInicio)
                .Take(10);
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
            return await GetSolicitudesActivasQuery(clienteId).ToListAsync();
        }

        private IQueryable<Solicitud> GetSolicitudesActivasQuery(int clienteId)
        {
            return _context.Solicitudes
                .Where(s => s.ClienteID == clienteId &&
                      (s.Estado == "Aceptada" || s.Estado == "En Progreso"))
                .Include(s => s.Cuidador)
                    .ThenInclude(c => c.Usuario);
        }

        public async Task<bool> CrearSolicitud(SolicitudServicioViewModel model, int clienteId)
        {
            try
            {
                var solicitud = CrearSolicitudEntity(model, clienteId);
                _context.Solicitudes.Add(solicitud);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private Solicitud CrearSolicitudEntity(SolicitudServicioViewModel model, int clienteId)
        {
            return new Solicitud
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