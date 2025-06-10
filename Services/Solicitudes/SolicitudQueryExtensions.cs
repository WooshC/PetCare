using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;

namespace PetCare.Services.Solicitudes
{
    public static class SolicitudQueryExtensions
    {
        public static IQueryable<Solicitud> IncludeClienteUsuario(this IQueryable<Solicitud> query)
        {
            return query.Include(s => s.Cliente)
                       .ThenInclude(c => c.Usuario);
        }

        public static async Task<IEnumerable<Solicitud>> GetSolicitudesByEstado(
            this ApplicationDbContext context,
            int userId,
            string[] estados,
            bool isCuidador = true,
            int? take = null)
        {
            var query = context.Solicitudes
                .IncludeClienteUsuario()
                .Where(s => isCuidador ? s.CuidadorID == userId : s.ClienteID == userId)
                .Where(s => estados.Contains(s.Estado));

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return await query.ToListAsync();
        }
    }
} 