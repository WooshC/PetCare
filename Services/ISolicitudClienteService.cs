using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Models;

namespace PetCare.Services
{
    public interface ISolicitudClienteService
    {
        Task<IEnumerable<Solicitud>> GetSolicitudesPendientes(int clienteId);
        Task<IEnumerable<Solicitud>> GetHistorialServicios(int clienteId);
        Task<bool> CancelarSolicitud(int solicitudId);
        Task<IEnumerable<Solicitud>> GetSolicitudesActivas(int clienteId);
        Task<IEnumerable<Cuidador>> GetCuidadoresDisponibles();
        Task<bool> CrearSolicitud(SolicitudServicioViewModel model, int clienteId);

    }
}