using System.ComponentModel.DataAnnotations;

namespace PetCare.Models
{
    public class Cuidador
    {
        public int CuidadorID { get; set; }

        [Required]
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(20)]
        public string DocumentoIdentidad { get; set; } = string.Empty;

        public byte[]? DocumentoIdentidadArchivo { get; set; }
        public byte[]? ComprobanteServiciosArchivo { get; set; }

        [Required]
        [StringLength(15)]
        public string TelefonoEmergencia { get; set; } = string.Empty;

        public string? Biografia { get; set; }
        public string? Experiencia { get; set; }
        public string? HorarioAtencion { get; set; }

        [Range(0, 9999999999.99)]
        public decimal? TarifaPorHora { get; set; }

        [Range(0, 5)]
        public decimal CalificacionPromedio { get; set; } = 0.0m;
        public bool DocumentoVerificado { get; set; } = false;
        public DateTime? FechaVerificacion { get; set; }

        [Required(ErrorMessage = "La ciudad es requerida")]
        [StringLength(100)]
        public string Ciudad { get; set; } = string.Empty;

        // Propiedades de navegación
        public Usuario Usuario { get; set; } = null!;
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
        public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
        public ICollection<DocumentoVerificacion> DocumentosVerificacion { get; set; } = new List<DocumentoVerificacion>();

        public void ActualizarPromedio()
        {
            CalificacionPromedio = Calificaciones.Any()
                ? (decimal)Calificaciones.Average(c => c.Puntuacion)
                : 0.0m;
        }
    }
}