using System.Collections.Generic;
using PetCare.Models;
using PetCare.Models.ViewModels;

namespace PetCare.Models.ViewModels
{
    public class CuidadorDashboardViewModel
    {
        public Cuidador Cuidador { get; set; }
        public IEnumerable<Solicitud> SolicitudesPendientes { get; set; }
        public IEnumerable<Solicitud> SolicitudesActivas { get; set; }
        public IEnumerable<Solicitud> SolicitudesEnProgreso { get; set; }
        public IEnumerable<Solicitud> HistorialServicios { get; set; }

        public decimal TotalGanado => HistorialServicios
           .Where(s => s.Estado == "Finalizada")
           .Sum(s => s.DuracionHoras * (Cuidador.TarifaPorHora ?? 0));
    }
}
