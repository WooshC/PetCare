using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;
using PetCare.Models.ViewModels;
using System.Security.Claims;

namespace PetCare.Services
{
    public class ClienteStrategy : IRoleStrategy
    {
        private readonly ApplicationDbContext _context;

        public ClienteStrategy(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> HandleRequestAsync(ApplicationDbContext context, Controller controller, Usuario usuario)
        {
            // Obtener el cliente asociado al usuario
            var cliente = await context.Clientes
                .Include(c => c.Usuario)
                .Include(c => c.Solicitudes)
                .ThenInclude(s => s.Cuidador)
                .ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.UsuarioID == usuario.UsuarioID);
  
            // Crear el ViewModel para el dashboard
            var viewModel = new ClienteDashboardViewModel
            {
                Cliente = cliente         
            };

            return controller.View("Dashboard", viewModel);
        }
        public async Task<IActionResult> HandleRequestAsync(Controller controller, Usuario usuario)
        {
            return await HandleRequestAsync(_context, controller, usuario);
        }
    }
}