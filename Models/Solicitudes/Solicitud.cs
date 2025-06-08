using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace PetCare.Models
{
    public class Solicitud
    {
        public int SolicitudID { get; set; }

        [Required]
        public int ClienteID { get; set; }

        public int? CuidadorID { get; set; }

        [Required]
        [StringLength(50)]
        public string TipoServicio { get; set; }

        [Required]
        public string Descripcion { get; set; }

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

        // Propiedades de navegación
        public Cliente Cliente { get; set; }
        public Cuidador? Cuidador { get; set; }

        [NotMapped]
        public List<Cuidador> CuidadoresRelacionados { get; set; } = new();
    }
}
