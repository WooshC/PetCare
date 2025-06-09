using Microsoft.AspNetCore.Mvc;
using PetCare.Data;
using PetCare.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace PetCare.Controllers
{
    public class RegistroController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistroController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegistroViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verificar si el usuario ya existe
                if (_context.Usuarios.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "El email ya está registrado");
                    return View(model);
                }

                if (_context.Usuarios.Any(u => u.NombreUsuario == model.NombreUsuario))
                {
                    ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está en uso");
                    return View(model);
                }

                // Crear nuevo usuario
                var usuario = new Usuario
                {
                    NombreUsuario = model.NombreUsuario,
                    ContrasenaHash = HashPassword(model.Password),
                    Email = model.Email,
                    NombreCompleto = model.NombreCompleto,
                    Telefono = model.Telefono,
                    Direccion = model.Direccion,
                    FechaRegistro = DateTime.Now,
                    Activo = true
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Asignar rol según el tipo de usuario
                int rolId = model.TipoUsuario == "Cuidador" ? 2 : 3;
                _context.UsuarioRoles.Add(new UsuarioRol
                {
                    UsuarioID = usuario.UsuarioID,
                    RolID = rolId,
                    FechaAsignacion = DateTime.Now
                });

                // Crear registro específico según el tipo de usuario
                if (model.TipoUsuario == "Cuidador")
                {
                    var cuidador = new Cuidador
                    {
                        UsuarioID = usuario.UsuarioID,
                        DocumentoIdentidad = model.DocumentoIdentidad,
                        TelefonoEmergencia = model.TelefonoEmergencia,
                        Biografia = model.Biografia,
                        Experiencia = model.Experiencia,
                        TarifaPorHora = model.TarifaPorHora,
                        Ciudad = model.Ciudad ?? "Ciudad no especificada"
                    };

                    // Procesar archivos subidos (ejemplo simplificado)
                    if (model.DocumentoIdentidadArchivo != null && model.DocumentoIdentidadArchivo.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await model.DocumentoIdentidadArchivo.CopyToAsync(memoryStream);
                        cuidador.DocumentoIdentidadArchivo = memoryStream.ToArray();
                    }

                    if (model.ComprobanteServiciosArchivo != null && model.ComprobanteServiciosArchivo.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await model.ComprobanteServiciosArchivo.CopyToAsync(memoryStream);
                        cuidador.ComprobanteServiciosArchivo = memoryStream.ToArray();
                    }

                    _context.Cuidadores.Add(cuidador);
                }
                else
                {
                    var cliente = new Cliente
                    {
                        UsuarioID = usuario.UsuarioID,
                        DocumentoIdentidad = model.DocumentoIdentidad
                    };

                    if (model.DocumentoIdentidadArchivo != null && model.DocumentoIdentidadArchivo.Length > 0)
                    {
                        using var memoryStream = new MemoryStream();
                        await model.DocumentoIdentidadArchivo.CopyToAsync(memoryStream);
                        cliente.DocumentoIdentidadArchivo = memoryStream.ToArray();
                    }

                    _context.Clientes.Add(cliente);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registro exitoso! Por favor inicia sesión.";
                return RedirectToAction("Index", "Login");
            }

            return View(model);
        }

        private string HashPassword(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma tu contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido")]
        public string Telefono { get; set; } = string.Empty;

        public string? Direccion { get; set; }

        [Required]
        public string TipoUsuario { get; set; } = "Cliente";

        // Campos comunes
        [Required(ErrorMessage = "El documento de identidad es requerido")]
        public string DocumentoIdentidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El archivo de identidad es requerido")]
        public IFormFile DocumentoIdentidadArchivo { get; set; } = null!;

        // Campos específicos para cuidadores
        public string? TelefonoEmergencia { get; set; }
        public IFormFile? ComprobanteServiciosArchivo { get; set; }
        public string? Biografia { get; set; }
        public string? Experiencia { get; set; }
        public decimal? TarifaPorHora { get; set; }
        [Required(ErrorMessage = "La ciudad es requerida")]
        [Display(Name = "Ciudad")]
        public string? Ciudad { get; set; }
    }
}