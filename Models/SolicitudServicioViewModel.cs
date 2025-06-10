using System.ComponentModel.DataAnnotations;

namespace PetCare.Models
{
    // Models/ViewModels/SolicitudServicioViewModel.cs
    public class SolicitudServicioViewModel
    {
        [Required]
        public int CuidadorID { get; set; }

        [Required]
        [Display(Name = "Tipo de Servicio")]
        public string TipoServicio { get; set; }

        [Required]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required]
        [Display(Name = "Fecha y Hora de Inicio")]
        public DateTime FechaHoraInicio { get; set; } = DateTime.Now.AddDays(1);

        [Required]
        [Display(Name = "Duración (horas)")]
        [Range(1, 168, ErrorMessage = "La duración debe ser entre 1 y 168 horas (1 semana)")]
        public int DuracionHoras { get; set; } = 2;

        [Required]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; }
    }
}