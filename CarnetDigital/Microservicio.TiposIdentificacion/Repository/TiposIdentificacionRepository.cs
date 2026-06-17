using Microservicio.TiposIdentificacion.Data;
using Microservicio.TiposIdentificacion.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.TiposIdentificacion.Repository
{
    public class TiposIdentificacionRepository : ITiposIdentificacionRepository
    {
        private readonly TiposIdentificacionDbContext _context;

        public TiposIdentificacionRepository(TiposIdentificacionDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoIdentificacion>> GetAllAsync()
        {
            return await _context.TiposIdentificacion
                .Where(t => t.Activo)
                .ToListAsync();
        }

        public async Task<TipoIdentificacion?> GetByIdAsync(int id)
        {
            return await _context.TiposIdentificacion
                .FirstOrDefaultAsync(t => t.Id == id && t.Activo);
        }

        public async Task<TipoIdentificacion> CreateAsync(TipoIdentificacion tipo)
        {
            _context.TiposIdentificacion.Add(tipo);
            await _context.SaveChangesAsync();
            return tipo;
        }

        public async Task<TipoIdentificacion?> UpdateAsync(int id, TipoIdentificacion tipo)
        {
            var existing = await _context.TiposIdentificacion
                .FirstOrDefaultAsync(t => t.Id == id && t.Activo);

            if (existing == null) return null;

            existing.Nombre = tipo.Nombre;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tipo = await _context.TiposIdentificacion
                .FirstOrDefaultAsync(t => t.Id == id && t.Activo);

            if (tipo == null) return false;

            tipo.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}