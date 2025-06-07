using System;
using System.ComponentModel.DataAnnotations;
namespace PetCare.Models.Solicitudes
{
 public class Solicitud
{
    public int SolicitudID { get; set; }

    [Required]
    public int ClienteID { get; set; }

    [Required]
    public int CuidadorID { get; set; }

    [Required]
    [StringLength(50)]
    public string TipoServicio { get; set; }

    [Required]
    public string Descripcion { get; set; }

    [Required]
    public DateTime FechaHoraInicio { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int DuracionHoras { get; set; }

    [Required]
    [StringLength(200)]
    public string Ubicacion { get; set; }

    [Required]
    [StringLength(20)]
    public string Estado { get; set; } = "Pendiente";

    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaActualizacion { get; set; }

    // Propiedades de navegación
    public Cliente Cliente { get; set; }
    public Cuidador Cuidador { get; set; }
}
}