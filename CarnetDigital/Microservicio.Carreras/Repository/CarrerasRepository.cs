using Microservicio.Carreras.Data;
using Microservicio.Carreras.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Carreras.Repository
{
    public class CarrerasRepository : ICarrerasRepository
    {
        private readonly CarrerasDbContext _context;

        public CarrerasRepository(CarrerasDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Carrera>> GetAllAsync()
        {
            return await _context.Carreras
                .Where(c => c.Activo)
                .ToListAsync();
        }

        public async Task<Carrera?> GetByIdAsync(int id)
        {
            return await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        }

        public async Task<Carrera> CreateAsync(Carrera carrera)
        {
            _context.Carreras.Add(carrera);
            await _context.SaveChangesAsync();
            return carrera;
        }

        public async Task<Carrera?> UpdateAsync(int id, Carrera carrera)
        {
            var existing = await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

            if (existing == null) return null;

            existing.Nombre = carrera.Nombre;
            existing.Director = carrera.Director;
            existing.Email = carrera.Email;
            existing.Telefono = carrera.Telefono;
            existing.IdInstitucion = carrera.IdInstitucion;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var carrera = await _context.Carreras
                .FirstOrDefaultAsync(c => c.Id == id && c.Activo);

            if (carrera == null) return false;

            carrera.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}