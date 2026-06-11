using Microservicio.Areas.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Areas.Data
{
    public class AreasDbContext : DbContext
    {
        public AreasDbContext(DbContextOptions<AreasDbContext> options) : base(options) { }

        public DbSet<AreaTrabajo> AreasTrabajo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AreaTrabajo>().ToTable("AreasTrabajo");
        }
    }
}