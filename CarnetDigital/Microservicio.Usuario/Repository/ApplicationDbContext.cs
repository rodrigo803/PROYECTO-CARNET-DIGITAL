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

            // ¡CORRECCIÓN APLICADA!
            // Ahora le decimos a Entity Framework que la Identificación es la Llave Primaria
            modelBuilder.Entity<Entities.Usuario>()
                .HasKey(u => u.Identificacion);
        }
    }
}
