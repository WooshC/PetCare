using System.Collections.Generic;
using PetCare.Models;
using PetCare.Models.ViewModels;

namespace PetCare.Models.ViewModels
{
    public class ClienteDashboardViewModel
    {
        public Cliente Cliente { get; set; }
        public IEnumerable<Solicitud> SolicitudesPendientes { get; set; }
        public IEnumerable<Solicitud> SolicitudesActivas { get; set; }
        public IEnumerable<Solicitud> SolicitudesEnProgreso { get; set; }
        public IEnumerable<Solicitud> HistorialServicios { get; set; }


    }
}