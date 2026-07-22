using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models.Areas;
using CarnetDigital.Frontend.Models.Shared;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.Areas;
using CarnetDigital.Frontend.Services.Instituciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class AreasController : Controller
    {
        private readonly IAreasApiService _areasApiService;
        private readonly IInstitucionesApiService _institucionesApiService;
        private readonly PaginacionOptions _paginacionOptions;

        public AreasController(
            IAreasApiService areasApiService,
            IInstitucionesApiService institucionesApiService,
            IOptions<PaginacionOptions> paginacionOptions)
        {
            _areasApiService = areasApiService;
            _institucionesApiService = institucionesApiService;
            _paginacionOptions = paginacionOptions.Value;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var todas = await _areasApiService.GetAllAsync();
            var tamanoPagina = _paginacionOptions.TamanoPagina;
            if (page < 1) page = 1;

            var resultado = new PagedResult<AreaDto>
            {
                PageNumber = page,
                PageSize = tamanoPagina,
                TotalItems = todas.Count,
                Items = todas
                    .OrderBy(a => a.Nombre)
                    .Skip((page - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList()
            };

            return View(resultado);
        }

        public async Task<IActionResult> Create()
        {
            var model = new AreaFormViewModel
            {
                Instituciones = await ObtenerInstitucionesSelectListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AreaFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Instituciones = await ObtenerInstitucionesSelectListAsync(model.IdInstitucion);
                return View(model);
            }

            var resultado = await _areasApiService.CreateAsync(model.Nombre.Trim(), model.IdInstitucion!.Value);
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                model.Instituciones = await ObtenerInstitucionesSelectListAsync(model.IdInstitucion);
                return View(model);
            }

            TempData["Success"] = $"El área de trabajo '{resultado.Data!.Nombre}' se creó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var area = await _areasApiService.GetByIdAsync(id);
            if (area is null)
                return NotFound();

            var model = new AreaFormViewModel
            {
                Id = area.Id,
                Nombre = area.Nombre,
                IdInstitucion = area.IdInstitucion,
                Instituciones = await ObtenerInstitucionesSelectListAsync(area.IdInstitucion)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AreaFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Id = id;
                model.Instituciones = await ObtenerInstitucionesSelectListAsync(model.IdInstitucion);
                return View(model);
            }

            var resultado = await _areasApiService.UpdateAsync(id, model.Nombre.Trim(), model.IdInstitucion!.Value);
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                model.Id = id;
                model.Instituciones = await ObtenerInstitucionesSelectListAsync(model.IdInstitucion);
                return View(model);
            }

            TempData["Success"] = $"El área de trabajo '{resultado.Data!.Nombre}' se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _areasApiService.DeleteAsync(id);
            TempData[eliminado ? "Success" : "Error"] = eliminado
                ? "El área de trabajo se eliminó correctamente."
                : "No se pudo eliminar el área de trabajo seleccionada.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> ObtenerInstitucionesSelectListAsync(int? seleccionadoId = null)
        {
            var instituciones = await _institucionesApiService.ObtenerActivasAsync();
            return instituciones
                .Select(i => new SelectListItem(i.Nombre, i.Id.ToString(), i.Id == seleccionadoId))
                .ToList();
        }
    }
}
