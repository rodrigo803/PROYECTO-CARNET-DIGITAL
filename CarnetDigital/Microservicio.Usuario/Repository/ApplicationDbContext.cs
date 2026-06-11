using Microservicio.Usuario.Entities;
using Microservicio.Usuario.Services;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Usuario.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Entities.Usuario> Usuarios { get; set; }
        public DbSet<EstadoUsuario> EstadoUsuario { get; set; }

        public DbSet<Institucion> Instituciones { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<UsuarioTelefono> UsuarioTelefonos { get; set; }
        public DbSet<UsuarioCarrera> UsuarioCarreras { get; set; }
        public DbSet<UsuarioArea> UsuarioAreas { get; set; }
        public DbSet<UsuarioInstitucion> UsuarioInstituciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Le decimos a Entity Framework que el Email es la llave primaria de Usuario
            modelBuilder.Entity<Entities.Usuario>()
                .HasKey(u => u.Email);

            base.OnModelCreating(modelBuilder);

            // 1. Configuramos las llaves compuestas para las tablas
            modelBuilder.Entity<UsuarioCarrera>()
                .HasKey(uc => new { uc.IdentificacionUsuario, uc.CarreraId });

            modelBuilder.Entity<UsuarioArea>()
                .HasKey(ua => new { ua.IdentificacionUsuario, ua.AreaId });

            modelBuilder.Entity<UsuarioInstitucion>()
                .HasKey(ui => new { ui.IdentificacionUsuario, ui.InstitucionId });

            // 2. Le decimos al modelo de Usuario que tiene colecciones
            modelBuilder.Entity<Entities.Usuario>()
                .HasMany(u => u.Telefonos)
                .WithOne(t => t.Usuario)
                .HasForeignKey(t => t.IdentificacionUsuario);

            modelBuilder.Entity<Entities.Usuario>()
                .HasMany(u => u.CarrerasAsociadas)
                .WithOne(c => c.Usuario)
                .HasForeignKey(c => c.IdentificacionUsuario);

            modelBuilder.Entity<Entities.Usuario>()
                .HasMany(u => u.AreasAsociadas)
                .WithOne(a => a.Usuario)
                .HasForeignKey(a => a.IdentificacionUsuario);

            modelBuilder.Entity<Entities.Usuario>()
                .HasMany(u => u.InstitucionesAsociadas)
                .WithOne(i => i.Usuario)
                .HasForeignKey(i => i.IdentificacionUsuario);
        }
    }
}
