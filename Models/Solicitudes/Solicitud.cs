using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PetCare.Models
{
    public class Solicitud
    {
        public int SolicitudID { get; set; }

        [Required]
        public int ClienteID { get; set; }

        [Required]
        public int CuidadorID { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoServicio { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public DateTime FechaHoraInicio { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DuracionHoras { get; set; }

        [Required]
        [StringLength(200)]
        public string Ubicacion { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal CostoTotal { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? FechaAceptacion { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? FechaInicioServicio { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? FechaFinalizacion { get; set; }

        // Propiedades calculadas
        [NotMapped]
        public bool EstaEnProgreso => Estado == "En Progreso";
        [NotMapped]
        public bool EstaFueraDeTiempo => Estado == "Fuera de Tiempo";
        [NotMapped]
        public bool PuedeFinalizar => EstaEnProgreso || EstaFueraDeTiempo;
        [NotMapped]
        public DateTime FechaFin => FechaHoraInicio.AddHours(DuracionHoras);
        [NotMapped]
        public bool EsValida => 
            DuracionHoras >= 1 && 
            DuracionHoras <= 24 && 
            FechaHoraInicio > DateTime.Now;

        // Propiedades de navegación
        public Cliente Cliente { get; set; }
        public Cuidador Cuidador { get; set; }

        [NotMapped]
        public List<Cuidador> CuidadoresRelacionados { get; set; } = new();

        // Método para calcular el costo total
        public void CalcularCostoTotal()
        {
            if (Cuidador != null && Cuidador.TarifaPorHora.HasValue)
            {
                CostoTotal = Cuidador.TarifaPorHora.Value * DuracionHoras;
            }
        }

        // Método para validar disponibilidad
        public bool ValidarDisponibilidad(ICollection<Solicitud> solicitudesExistentes)
        {
            if (!EsValida) return false;

            return !solicitudesExistentes.Any(s => 
                s.CuidadorID == CuidadorID && 
                s.Estado != "Cancelada" &&
                ((FechaHoraInicio >= s.FechaHoraInicio && FechaHoraInicio < s.FechaFin) ||
                 (FechaFin > s.FechaHoraInicio && FechaFin <= s.FechaFin) ||
                 (FechaHoraInicio <= s.FechaHoraInicio && FechaFin >= s.FechaFin)));
        }
    }
}
