using Microservicio.Usuario.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json;

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

            // 1. Configuración de Listas como JSON (La forma infalible)
            // Esto convierte la List<int> a string JSON para guardarla en SQL Server
            modelBuilder.Entity<Entities.Usuario>()
                .Property(u => u.InstitucionesIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null) ?? new List<int>());

            modelBuilder.Entity<Entities.Usuario>()
                .Property(u => u.CarrerasIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null) ?? new List<int>());

            modelBuilder.Entity<Entities.Usuario>()
                .Property(u => u.AreasIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null) ?? new List<int>());

            // 2. Llave Primaria
            modelBuilder.Entity<Entities.Usuario>()
                .HasKey(u => u.Identificacion);
        }
    }
}