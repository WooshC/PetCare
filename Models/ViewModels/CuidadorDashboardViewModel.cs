using System.Collections.Generic;
using PetCare.Models;
using PetCare.Models.Solicitudes;

namespace PetCare.Models.ViewModels
{
    public class CuidadorDashboardViewModel
    {
        public Cuidador Cuidador { get; set; }
        public IEnumerable<Solicitud> SolicitudesPendientes { get; set; }
        public IEnumerable<Solicitud> HistorialServicios { get; set; }
    }
}