using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare.Models
{
    public class DocumentoVerificacion
    {
        [Key]
        public int DocumentoID { get; set; }

        [Required]
        public int CuidadorID { get; set; } // Cambiado de UsuarioID a CuidadorID

        [Required]
        [StringLength(50)]
        public string TipoDocumento { get; set; } = string.Empty;

        [Required]
        public byte[] Archivo { get; set; } = Array.Empty<byte>();

        public DateTime FechaSubida { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public string? Comentarios { get; set; }
        public DateTime? FechaVerificacion { get; set; }

        // Propiedad de navegación
        [ForeignKey("CuidadorID")]
        public Cuidador Cuidador { get; set; } = null!; // Cambiado de Usuario a Cuidador
    }
}