using Microservicio.Instituciones.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Instituciones.Data
{
    public class InstitucionesDbContext : DbContext
    {
        public InstitucionesDbContext(DbContextOptions<InstitucionesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Institucion> Instituciones { get; set; }
        public DbSet<InstitucionDominio> InstitucionDominios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración: Institución → Dominios (1 a muchos)
            modelBuilder.Entity<InstitucionDominio>()
                .HasOne(d => d.Institucion)
                .WithMany(i => i.Dominios)
                .HasForeignKey(d => d.IdInstitucion)
                .OnDelete(DeleteBehavior.Cascade);

            // Nombres de tabla explícitos
            modelBuilder.Entity<Institucion>().ToTable("Instituciones");
            modelBuilder.Entity<InstitucionDominio>().ToTable("InstitucionDominios");
        }
    }
}