using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Models;
using PetCare.Models.ViewModels;

namespace PetCare.Services
{
    public interface ISolicitudService
    {
        Task<IEnumerable<Solicitud>> GetSolicitudesPendientes(int cuidadorId);
        Task<IEnumerable<Solicitud>> GetHistorialServicios(int cuidadorId);
        Task<IEnumerable<Solicitud>> GetSolicitudesActivas(int cuidadorId);
        Task<bool> CambiarEstadoSolicitud(int solicitudId, string nuevoEstado);
        Task ActualizarEstadosAutomaticos();
        Task<bool> FinalizarSolicitud(int solicitudId, int clienteId); 
    }
}