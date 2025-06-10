using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;
using PetCare.Services;

public class CuidadorService : ICuidadorService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CuidadorService> _logger;

    public CuidadorService(ApplicationDbContext context, ILogger<CuidadorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Cuidador>> GetCuidadoresDisponibles()
    {
        try
        {
            _logger.LogInformation("Obteniendo cuidadores disponibles");

            var cuidadores = await _context.Cuidadores
                .Include(c => c.Usuario)
                .Include(c => c.Calificaciones)
                .Where(c => c.Disponible && c.DocumentoVerificado)
                .OrderByDescending(c => c.CalificacionPromedio)
                .ToListAsync();

            _logger.LogInformation($"Encontrados {cuidadores.Count} cuidadores disponibles");

            return cuidadores;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener cuidadores disponibles");
            throw;
        }
    }
}