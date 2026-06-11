using Microservicio.TiposUsuario.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.TiposUsuario.Data
{
    public class TiposUsuarioDbContext : DbContext
    {
        public TiposUsuarioDbContext(DbContextOptions<TiposUsuarioDbContext> options) : base(options) { }

        public DbSet<TipoUsuario> TiposUsuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TipoUsuario>().ToTable("TiposUsuario");
        }
    }
}