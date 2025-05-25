using System;
using System.ComponentModel.DataAnnotations;

namespace PetCare.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreUsuario { get; set; }

        [Required]
        public string ContrasenaHash { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(15)]
        public string Telefono { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public DateTime? UltimoAcceso { get; set; }
        public bool Activo { get; set; } = true;

        // Propiedades de navegación
        public ICollection<UsuarioRol> Roles { get; set; }
    }
}