using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;
using PetCare.Services;

namespace PetCare.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly RoleStrategyFactory _roleStrategyFactory;

        public BaseController(ApplicationDbContext context, RoleStrategyFactory roleStrategyFactory)
        {
            _context = context;
            _roleStrategyFactory = roleStrategyFactory;
        }

        protected async Task<IActionResult> RedirectByRole(Usuario usuario)
        {
            // Obtener roles del usuario
            var roles = await _context.UsuarioRoles
                .Include(ur => ur.Rol)
                .Where(ur => ur.UsuarioID == usuario.UsuarioID)
                .Select(ur => ur.Rol.NombreRol)
                .ToListAsync();

            // Tomar el primer rol (podrías implementar lógica más compleja si es necesario)
            var primaryRole = roles.FirstOrDefault();

            if (primaryRole == null)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var strategy = _roleStrategyFactory.CreateStrategy(primaryRole);
            return await strategy.HandleRequestAsync(_context, this, usuario);
        }
    }
}
