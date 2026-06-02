using System;
using System.Collections.Generic;
using System.Text;
using CarnetDigital.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarnetDigital.Data.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Esta propiedad representa la tabla en la base de datos
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones específicas de las tablas (Fluent API)
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.Email); // Define el email como llave primaria
        }
    }
}
