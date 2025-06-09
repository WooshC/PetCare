using System.ComponentModel.DataAnnotations;

namespace PetCare.Models.ViewModels
{
    public class NuevaSolicitudViewModel
    {
        [Required]
        [StringLength(50)]
        public string TipoServicio { get; set; }

        [Required]
        [StringLength(200)]
        public string Ubicacion { get; set; }

        [Required]
        [Range(1, 200)]
        public int DuracionHoras { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Display(Name = "Hora deseada")]
        [DataType(DataType.Time)]
        public TimeSpan? HoraDeseada { get; set; }

    }
}
