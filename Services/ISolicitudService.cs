using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Models;
using PetCare.Models.ViewModels;

namespace PetCare.Services
{
    public interface ISolicitudService : IBaseSolicitudService
    {
        Task<bool> CambiarEstadoSolicitud(int solicitudId, string nuevoEstado);
        Task<IEnumerable<Solicitud>> GetSolicitudesFueraDeTiempo(int cuidadorId);
    }
}