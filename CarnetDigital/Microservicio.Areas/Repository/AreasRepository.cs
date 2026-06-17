using Microservicio.Areas.Data;
using Microservicio.Areas.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Areas.Repository
{
    public class AreasRepository : IAreasRepository
    {
        private readonly AreasDbContext _context;

        public AreasRepository(AreasDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AreaTrabajo>> GetAllAsync()
        {
            return await _context.AreasTrabajo
                .Where(a => a.Activo)
                .ToListAsync();
        }

        public async Task<AreaTrabajo?> GetByIdAsync(int id)
        {
            return await _context.AreasTrabajo
                .FirstOrDefaultAsync(a => a.Id == id && a.Activo);
        }

        public async Task<AreaTrabajo> CreateAsync(AreaTrabajo area)
        {
            _context.AreasTrabajo.Add(area);
            await _context.SaveChangesAsync();
            return area;
        }

        public async Task<AreaTrabajo?> UpdateAsync(int id, AreaTrabajo area)
        {
            var existing = await _context.AreasTrabajo
                .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

            if (existing == null) return null;

            existing.Nombre = area.Nombre;
            existing.IdInstitucion = area.IdInstitucion;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var area = await _context.AreasTrabajo
                .FirstOrDefaultAsync(a => a.Id == id && a.Activo);

            if (area == null) return false;

            area.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}