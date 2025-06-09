using System;
using System.ComponentModel.DataAnnotations;

namespace PetCare.Models
{
    public class Reserva
    {
        [Key]
        public int ReservaId { get; set; }
        
        public int ClienteId { get; set; }
        public int CuidadorId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } // Pendiente, Aceptado, Rechazado, Iniciado, Finalizado
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public string Comentarios { get; set; }
        
        // Relaciones
        public virtual Cliente Cliente { get; set; }
        public virtual Cuidador Cuidador { get; set; }
    }
} 