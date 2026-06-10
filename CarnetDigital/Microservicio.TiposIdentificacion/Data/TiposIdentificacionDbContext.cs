using Microservicio.TiposIdentificacion.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.TiposIdentificacion.Data
{
    public class TiposIdentificacionDbContext : DbContext
    {
        public TiposIdentificacionDbContext(DbContextOptions<TiposIdentificacionDbContext> options) : base(options) { }

        public DbSet<TipoIdentificacion> TiposIdentificacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TipoIdentificacion>().ToTable("TiposIdentificacion");
        }
    }
}