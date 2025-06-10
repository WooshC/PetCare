using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Models;

namespace PetCare.Services
{
    public interface IBaseSolicitudService
    {
        Task<IEnumerable<Solicitud>> GetSolicitudesPendientes(int userId);
        Task<IEnumerable<Solicitud>> GetHistorialServicios(int userId);
        Task<IEnumerable<Solicitud>> GetSolicitudesActivas(int userId);
    }
} 