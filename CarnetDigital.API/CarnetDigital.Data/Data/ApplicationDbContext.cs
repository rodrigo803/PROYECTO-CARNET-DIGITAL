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

        // Estas son las tablas de tu microservicio
        public DbSet<Usuario> Usuarios { get; set; }
        // Si creaste la entidad EstadoUsuario en Core/Entities, la agregas aquí también:
        // public DbSet<EstadoUsuario> EstadoUsuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Le decimos a Entity Framework que el Email es la llave primaria de Usuario
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.Email);
        }
    }
}
