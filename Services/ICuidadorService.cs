using System.Collections.Generic;
using System.Threading.Tasks;
using PetCare.Models;

namespace PetCare.Services
{
    public interface ICuidadorService
    {
        Task<IEnumerable<Cuidador>> GetCuidadoresDisponibles();
    }
}