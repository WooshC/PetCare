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

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRoles { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cuidador> Cuidadores { get; set; }
        public DbSet<DocumentoVerificacion> DocumentosVerificacion { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }

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

            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasOne(c => c.Usuario)
                      .WithOne(u => u.Cliente)
                      .HasForeignKey<Cliente>(c => c.UsuarioID);
            });

            // Configuración de Cuidador
            modelBuilder.Entity<Cuidador>(entity =>
            {
                entity.HasOne(c => c.Usuario)
                      .WithOne(u => u.Cuidador)
                      .HasForeignKey<Cuidador>(c => c.UsuarioID);

                entity.Property(c => c.TarifaPorHora).HasPrecision(10, 2);
                entity.Property(c => c.CalificacionPromedio).HasPrecision(3, 2);
            });

            // Configuración de Calificacion
            modelBuilder.Entity<Calificacion>(entity =>
            {
                entity.HasOne(c => c.Cuidador)
                      .WithMany(c => c.Calificaciones)
                      .HasForeignKey(c => c.CuidadorID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Cliente)
                      .WithMany(c => c.Calificaciones)
                      .HasForeignKey(c => c.ClienteID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.FechaCalificacion)
                      .HasDefaultValueSql("GETDATE()");
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
                    ContrasenaHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                    Email = "admin@petcare.com",
                    NombreCompleto = "Administrador del Sistema",
                    Telefono = "0998887777",
                    Direccion = "Calle Principal 123",
                    Activo = true,
                    FechaRegistro = new DateTime(2024, 01, 01, 0, 0, 0)
                }
            );

            modelBuilder.Entity<UsuarioRol>().HasData(
                new UsuarioRol
                {
                    UsuarioRolID = 1,
                    UsuarioID = 1,
                    RolID = 1
                }
            );

            modelBuilder.Entity<Calificacion>().HasData(
      new Calificacion
      {
          CalificacionID = 1,
          CuidadorID = 1,
          ClienteID = 1,
          Puntuacion = 5,
          Comentario = "Excelente servicio con mi mascota",
          FechaCalificacion = new DateTime(2025, 6, 6, 0, 0, 0) 
      }
  );

        }
    }
}
