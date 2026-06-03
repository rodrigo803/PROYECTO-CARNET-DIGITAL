using CarnetDigital.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarnetDigital.Data.Data // Usa el namespace que tengas configurado aquí
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // ¡AQUÍ ESTÁ LA MAGIA! Esto quita la línea roja de _context.Usuarios
        public DbSet<Usuario> Usuarios { get; set; }

        // Y esto quita la línea roja de _context.EstadoUsuario
        public DbSet<EstadoUsuario> EstadoUsuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Le decimos a Entity Framework que el Email es la llave primaria de Usuario
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.Email);
        }
    }
}