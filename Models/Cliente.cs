using PetCare.Models.Solicitudes;
using System.ComponentModel.DataAnnotations;

namespace PetCare.Models
{
    public class Cliente
    {
        public int ClienteID { get; set; }

        [Required]
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(20)]
        public string DocumentoIdentidad { get; set; } = string.Empty;

        public byte[]? DocumentoIdentidadArchivo { get; set; }
        public bool DocumentoVerificado { get; set; } = false;
        public DateTime? FechaVerificacion { get; set; }

        // Propiedad de navegación
        public Usuario Usuario { get; set; } = null!;
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
        public ICollection<Solicitud> Solicitudes { get; set; } = new List<Solicitud>();
    }
}