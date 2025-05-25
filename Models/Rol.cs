using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetCare.Models
{
    public class Rol
    {
        public int RolID { get; set; }

        [Required]
        [StringLength(30)]
        public string NombreRol { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        // Propiedad de navegación
        public ICollection<UsuarioRol> Usuarios { get; set; }
    }
}