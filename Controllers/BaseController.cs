using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCare.Data;
using PetCare.Models;
using PetCare.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace PetCare.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly RoleStrategyFactory _roleStrategyFactory;

        public BaseController(ApplicationDbContext context, RoleStrategyFactory roleStrategyFactory)
        {
            _context = context;
            _roleStrategyFactory = roleStrategyFactory;
        }

        // Versión mejorada de RedirectByRole
        protected async Task<IActionResult> RedirectByRole(Usuario usuario = null)
        {
            // Verificación de autenticación
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Login");
            }

            // Obtener usuario si no se proporciona
            if (usuario == null)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                usuario = await _context.Usuarios
                    .Include(u => u.Roles)
                    .ThenInclude(ur => ur.Rol)
                    .FirstOrDefaultAsync(u => u.UsuarioID == userId);

                if (usuario == null)
                {
                    await HttpContext.SignOutAsync();
                    return RedirectToAction("Index", "Login");
                }
            }

            // Obtener roles
            var roles = await _context.UsuarioRoles
                .Include(ur => ur.Rol)
                .Where(ur => ur.UsuarioID == usuario.UsuarioID)
                .Select(ur => ur.Rol.NombreRol)
                .ToListAsync();

            // Redirección basada en roles con estrategia de fallback
            try
            {
                if (roles.Count == 0)
                {
                    return RedirectToAction("AccessDenied", "Home");
                }

                // Prioridad de roles si el usuario tiene múltiples
                if (roles.Contains("Administrador"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("Cuidador"))
                {
                    return RedirectToAction("Dashboard", "Cuidador");
                }
                else if (roles.Contains("Cliente"))
                {
                    return RedirectToAction("Dashboard", "Cliente");
                }

                // Fallback al sistema de estrategias si no coincide con los roles principales
                var primaryRole = roles.First();
                var strategy = _roleStrategyFactory.CreateStrategy(primaryRole);
                return await strategy.HandleRequestAsync(_context, this, usuario);
            }
            catch (NotImplementedException)
            {
                // Si no hay estrategia para el rol, redirigir a acceso denegado
                return RedirectToAction("AccessDenied", "Home");
            }
        }

        // Método auxiliar para obtener el usuario actual
        protected async Task<Usuario> GetCurrentUserAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return await _context.Usuarios
                .Include(u => u.Roles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioID == userId);
        }
    }
}