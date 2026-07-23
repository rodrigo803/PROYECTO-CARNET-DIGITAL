using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models.Instituciones;
using CarnetDigital.Frontend.Models.Shared;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.Instituciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class InstitucionesController : Controller
    {
        private readonly IInstitucionesCrudApiService _institucionesApiService;
        private readonly PaginacionOptions _paginacionOptions;

        public InstitucionesController(
            IInstitucionesCrudApiService institucionesApiService,
            IOptions<PaginacionOptions> paginacionOptions)
        {
            _institucionesApiService = institucionesApiService;
            _paginacionOptions = paginacionOptions.Value;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var todas = await _institucionesApiService.GetAllAsync();
            var tamanoPagina = _paginacionOptions.TamanoPagina;
            if (page < 1) page = 1;

            var resultado = new PagedResult<InstitucionDetalleDto>
            {
                PageNumber = page,
                PageSize = tamanoPagina,
                TotalItems = todas.Count,
                Items = todas
                    .OrderBy(i => i.Nombre)
                    .Skip((page - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList()
            };

            return View(resultado);
        }

        public IActionResult Create()
        {
            return View(new InstitucionFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstitucionFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dominios = DominiosTextoHelper.Parse(model.DominiosTexto);
            var resultado = await _institucionesApiService.CreateAsync(model.Nombre.Trim(), model.Email.Trim(), model.Telefono.Trim(), dominios);

            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                return View(model);
            }

            TempData["Success"] = $"La institución '{resultado.Data!.Nombre}' se creó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var institucion = await _institucionesApiService.GetByIdAsync(id);
            if (institucion is null)
                return NotFound();

            var model = new InstitucionFormViewModel
            {
                Id = institucion.Id,
                Nombre = institucion.Nombre,
                Email = institucion.Email,
                Telefono = institucion.Telefono,
                DominiosTexto = string.Join("\n", institucion.Dominios.Select(d => d.Dominio))
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InstitucionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Id = id;
                return View(model);
            }

            var dominios = DominiosTextoHelper.Parse(model.DominiosTexto);
            var resultado = await _institucionesApiService.UpdateAsync(id, model.Nombre.Trim(), model.Email.Trim(), model.Telefono.Trim(), dominios);

            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                model.Id = id;
                return View(model);
            }

            TempData["Success"] = $"La institución '{resultado.Data!.Nombre}' se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _institucionesApiService.DeleteAsync(id);
            TempData[eliminado ? "Success" : "Error"] = eliminado
                ? "La institución se eliminó correctamente."
                : "No se pudo eliminar la institución seleccionada.";

            return RedirectToAction(nameof(Index));
        }
    }
}
