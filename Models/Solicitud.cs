using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string TipoServicio { get; set; } = string.Empty;

        [Required]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        public DateTime FechaHoraInicio { get; set; }

        [Required]
        public int DuracionHoras { get; set; }

        [Required]
        public string Ubicacion { get; set; } = string.Empty;

        [Required]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Aceptada, Rechazada, Finalizada

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaActualizacion { get; set; }

        // Relaciones
        public Cliente Cliente { get; set; } = null!;
        public Cuidador Cuidador { get; set; } = null!;

        [NotMapped]
        public List<Cuidador> CuidadoresRelacionados { get; set; } = new();
    }
}
