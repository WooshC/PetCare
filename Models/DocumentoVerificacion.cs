using System;
using System.ComponentModel.DataAnnotations;

namespace PetCare.Models
{
    public class DocumentoVerificacion
    {
        [Key]
        public int DocumentoID { get; set; }

        [Required]
        public int UsuarioID { get; set; }

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
        public Usuario Usuario { get; set; } = null!;
    }
}