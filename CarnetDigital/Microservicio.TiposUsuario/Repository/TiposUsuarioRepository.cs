using Microservicio.TiposUsuario.Data;
using Microservicio.TiposUsuario.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.TiposUsuario.Repository
{
    public class TiposUsuarioRepository : ITiposUsuarioRepository
    {
        private readonly TiposUsuarioDbContext _context;

        public TiposUsuarioRepository(TiposUsuarioDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoUsuario>> GetAllAsync()
        {
            return await _context.TiposUsuario
                .Where(t => t.Activo)
                .ToListAsync();
        }

        public async Task<TipoUsuario?> GetByIdAsync(int id)
        {
            return await _context.TiposUsuario
                .FirstOrDefaultAsync(t => t.Id == id && t.Activo);
        }

        public async Task<TipoUsuario> CreateAsync(TipoUsuario tipo)
        {
            _context.TiposUsuario.Add(tipo);
            await _context.SaveChangesAsync();
            return tipo;
        }

        public async Task<TipoUsuario?> UpdateAsync(int id, TipoUsuario tipo)
        {
            var existing = await _context.TiposUsuario
                .FirstOrDefaultAsync(t => t.Id == id && t.Activo);

            if (existing == null) return null;

            existing.Nombre = tipo.Nombre;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var tipo = await _context.TiposUsuario
                .FirstOrDefaultAsync(t => t.Id == id && t.Activo);

            if (tipo == null) return false;

            tipo.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}