using Microservicio.Carreras.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Carreras.Data
{
    public class CarrerasDbContext : DbContext
    {
        public CarrerasDbContext(DbContextOptions<CarrerasDbContext> options)
            : base(options)
        {
        }

        public DbSet<Carrera> Carreras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Carrera>().ToTable("Carreras");
        }
    }
}