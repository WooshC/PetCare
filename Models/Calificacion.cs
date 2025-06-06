using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare.Models
{
    public class Calificacion
    {
        public int CalificacionID { get; set; }

        [ForeignKey("Cuidador")]
        public int CuidadorID { get; set; }
        public Cuidador Cuidador { get; set; }

        [ForeignKey("Cliente")]
        public int ClienteID { get; set; }
        public Cliente Cliente { get; set; }

        [Range(1, 5)]
        public int Puntuacion { get; set; }

        [StringLength(500)]
        public string Comentario { get; set; }

        public DateTime FechaCalificacion { get; set; } = DateTime.Now;
    }
}