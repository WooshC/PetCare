using System;
using System.ComponentModel.DataAnnotations;

namespace PetCare.Models
{
    public class Calificacion
    {
        public int CalificacionID { get; set; }

        [Required]
        public int CuidadorID { get; set; }

        [Required]
        public int ClienteID { get; set; }

        [Required]
        [Range(1, 5)]
        public int Puntuacion { get; set; }

        public string? Comentario { get; set; }

        public DateTime FechaCalificacion { get; set; } 

        // Propiedades de navegación
        public Cuidador Cuidador { get; set; } = null!;
        public Cliente Cliente { get; set; } = null!;
    }
}