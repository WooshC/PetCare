using Microsoft.EntityFrameworkCore;
using PetCare.Models;

namespace PetCare.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Solo las tablas básicas para login y administración
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.NombreUsuario).IsUnique();
                entity.Property(u => u.Activo).HasDefaultValue(true);
                entity.Property(u => u.FechaRegistro).HasDefaultValueSql("GETDATE()");
            });

            // Configuración de UsuarioRol
            modelBuilder.Entity<UsuarioRol>(entity =>
            {
                entity.HasIndex(ur => new { ur.UsuarioID, ur.RolID }).IsUnique();

                entity.HasOne(ur => ur.Usuario)
                      .WithMany(u => u.Roles)
                      .HasForeignKey(ur => ur.UsuarioID);

                entity.HasOne(ur => ur.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(ur => ur.RolID);

                entity.Property(ur => ur.FechaAsignacion).HasDefaultValueSql("GETDATE()");
            });

            // Datos iniciales
            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolID = 1, NombreRol = "Administrador", Descripcion = "Acceso completo al sistema" },
                new Rol { RolID = 2, NombreRol = "Cuidador", Descripcion = "Personas que ofrecen servicios de cuidado de mascotas" },
                new Rol { RolID = 3, NombreRol = "Cliente", Descripcion = "Dueños de mascotas que buscan servicios" }
            );

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    UsuarioID = 1,
                    NombreUsuario = "admin",
                    ContrasenaHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", // SHA256 de 'admin'
                    Email = "admin@petcare.com",
                    NombreCompleto = "Administrador del Sistema",
                    Telefono = "0998887777",
                    Direccion = "Calle Principal 123",
                    Activo = true
                }
            );

            modelBuilder.Entity<UsuarioRol>().HasData(
                new UsuarioRol { UsuarioRolID = 1, UsuarioID = 1, RolID = 1 }
            );
        }
    }
}