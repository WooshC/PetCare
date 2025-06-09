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

                // Validación específica para tipo de usuario
                if (model.TipoUsuario == "Cuidador" && model.ComprobanteServiciosArchivo == null)
                {
                    ModelState.AddModelError("ComprobanteServiciosArchivo", "El comprobante de servicios es requerido para cuidadores");
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
                int rolId = model.TipoUsuario == "Cuidador" ? 2 : 3; // Asegúrate que estos IDs coincidan con tu base de datos
                _context.UsuarioRoles.Add(new UsuarioRol
                {
                    UsuarioID = usuario.UsuarioID,
                    RolID = rolId,
                    FechaAsignacion = DateTime.Now
                });

                // Manejo de archivos
                byte[] documentoIdentidadBytes = null;
                if (model.DocumentoIdentidadArchivo != null && model.DocumentoIdentidadArchivo.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await model.DocumentoIdentidadArchivo.CopyToAsync(memoryStream);
                    documentoIdentidadBytes = memoryStream.ToArray();
                }

                // Crear registro específico según el tipo de usuario
                if (model.TipoUsuario == "Cuidador")
                {
                    if (string.IsNullOrEmpty(model.Ciudad))
                    {
                        ModelState.AddModelError("Ciudad", "La ciudad es requerida para cuidadores");
                        return View(model);
                    }

                    var cuidador = new Cuidador
                    {
                        UsuarioID = usuario.UsuarioID,
                        DocumentoIdentidad = model.DocumentoIdentidad,
                        DocumentoIdentidadArchivo = documentoIdentidadBytes,
                        TelefonoEmergencia = model.TelefonoEmergencia,
                        Biografia = model.Biografia,
                        Experiencia = model.Experiencia,
                        TarifaPorHora = model.TarifaPorHora ?? 0,
                        Ciudad = model.Ciudad
                    };

                    // Procesar archivo de comprobante solo para cuidadores
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
                        DocumentoIdentidad = model.DocumentoIdentidad,
                        DocumentoIdentidadArchivo = documentoIdentidadBytes
                    };

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

        [Required(ErrorMessage = "Seleccione un tipo de usuario")]
        public string TipoUsuario { get; set; } = "Cliente";

        // Campos comunes
        [Required(ErrorMessage = "El documento de identidad es requerido")]
        public string DocumentoIdentidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El archivo de identidad es requerido")]
        public IFormFile DocumentoIdentidadArchivo { get; set; } = null!;

        // Campos específicos para cuidadores
        public string? TelefonoEmergencia { get; set; }

        [Display(Name = "Comprobante de servicios (solo cuidadores)")]
        public IFormFile? ComprobanteServiciosArchivo { get; set; }

        public string? Biografia { get; set; }
        public string? Experiencia { get; set; }

        [Range(0, 100, ErrorMessage = "La tarifa debe estar entre 0 y 100")]
        public decimal? TarifaPorHora { get; set; }

        [Display(Name = "Ciudad (solo cuidadores)")]
        public string? Ciudad { get; set; }
    }
}