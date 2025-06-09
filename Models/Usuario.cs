using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare.Models
{
    public class Usuario
    {
        [Key]
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
        public bool Verificado { get; set; } = false;

        // Propiedades de navegación
        public ICollection<UsuarioRol> Roles { get; set; } = new List<UsuarioRol>();
        
        [InverseProperty("Usuario")]
        public Cliente? Cliente { get; set; }
        
        [InverseProperty("Usuario")]
        public Cuidador? Cuidador { get; set; }
        
        public ICollection<DocumentoVerificacion> DocumentosVerificacion { get; set; } = new List<DocumentoVerificacion>();
    }
}