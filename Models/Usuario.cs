using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetCare.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        public string ContrasenaHash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Direccion { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime? UltimoAcceso { get; set; }
        public bool Activo { get; set; } = true;

        // Propiedades de navegación
        public ICollection<UsuarioRol> Roles { get; set; } = new List<UsuarioRol>();
        public Cliente? Cliente { get; set; }
        public Cuidador? Cuidador { get; set; }
    }
}