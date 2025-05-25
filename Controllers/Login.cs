using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using PetCare.Models;

namespace PetCare.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            string hashedPassword = HashPassword(password);

            var usuario = _context.Usuarios
                .Where(u => u.Email == email && u.ContrasenaHash == hashedPassword && u.Activo)
                .Select(u => new
                {
                    u.UsuarioID,
                    u.NombreCompleto,
                    u.Email,
                    u.Telefono,
                    u.Direccion,
                    Roles = u.Roles.Select(r => r.Rol.NombreRol)
                })
                .FirstOrDefault();

            if (usuario == null)
            {
                ViewBag.Error = "Credenciales inválidas";
                return View();
            }

            // Pasar los datos a la vista Welcome
            return View("Welcome", usuario);
        }

        private string HashPassword(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
