using Microservicio.Instituciones.Data;
using Microservicio.Instituciones.Entities;
using Microsoft.EntityFrameworkCore;

namespace Microservicio.Instituciones.Repository
{
    public class InstitucionesRepository : IInstitucionesRepository
    {
        private readonly InstitucionesDbContext _context;

        public InstitucionesRepository(InstitucionesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Institucion>> GetAllAsync()
        {
            return await _context.Instituciones
                .Where(i => i.Activo)
                .Include(i => i.Dominios.Where(d => d.Activo))
                .ToListAsync();
        }

        public async Task<Institucion?> GetByIdAsync(int id)
        {
            return await _context.Instituciones
                .Where(i => i.Id == id && i.Activo)
                .Include(i => i.Dominios.Where(d => d.Activo))
                .FirstOrDefaultAsync();
        }

        public async Task<Institucion> CreateAsync(Institucion institucion)
        {
            _context.Instituciones.Add(institucion);
            await _context.SaveChangesAsync();
            return institucion;
        }

        public async Task<Institucion?> UpdateAsync(int id, Institucion institucion)
        {
            var existing = await _context.Instituciones
                .Include(i => i.Dominios)
                .FirstOrDefaultAsync(i => i.Id == id && i.Activo);

            if (existing == null) return null;

            existing.Nombre = institucion.Nombre;
            existing.Email = institucion.Email;
            existing.Telefono = institucion.Telefono;

            _context.InstitucionDominios.RemoveRange(existing.Dominios);

            existing.Dominios = institucion.Dominios
                .Select(d => new InstitucionDominio
                {
                    Dominio = d.Dominio.Trim(),
                    Activo = true,
                    IdInstitucion = id
                })
                .ToList();

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var institucion = await _context.Instituciones
                .Include(i => i.Dominios)
                .FirstOrDefaultAsync(i => i.Id == id && i.Activo);

            if (institucion == null) return false;

            institucion.Activo = false;
            foreach (var dominio in institucion.Dominios)
                dominio.Activo = false;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}