using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using PetCare.Models;
using PetCare.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace PetCare.Controllers
{
    public class LoginController : BaseController
    {
        public LoginController(
        ApplicationDbContext context,
        RoleStrategyFactory roleStrategyFactory)
        : base(context, roleStrategyFactory)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Por favor ingrese email y contraseña";
                return View();
            }

            string hashedPassword = HashPassword(password);

            var usuario = await _context.Usuarios
                .Include(u => u.Roles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Email == email && u.ContrasenaHash == hashedPassword && u.Activo);

            if (usuario == null)
            {
                ViewBag.Error = "Credenciales inválidas";
                return View();
            }

            // Crear la identidad del usuario (claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioID.ToString()),
                new Claim("FullName", usuario.NombreCompleto),
                new Claim(ClaimTypes.Email, usuario.Email)
            };

            // Agregar roles como claims
            foreach (var rol in usuario.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol.Rol.NombreRol));
            }

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                // Configuraciones adicionales si son necesarias
                IsPersistent = true // Para mantener la sesión
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Actualizar último acceso
            usuario.UltimoAcceso = DateTime.Now;
            await _context.SaveChangesAsync();

            // Redirigir según el rol usando la lógica del BaseController
            return await RedirectByRole(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }


        private string HashPassword(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}