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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Le decimos a Entity Framework que el Email es la llave primaria de Usuario
            modelBuilder.Entity<Entities.Usuario>()
                .HasKey(u => u.Email);

            base.OnModelCreating(modelBuilder);
        }
    }
}
