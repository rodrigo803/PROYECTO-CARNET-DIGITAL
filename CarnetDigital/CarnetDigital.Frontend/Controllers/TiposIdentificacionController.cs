using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models.Shared;
using CarnetDigital.Frontend.Models.TiposIdentificacion;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.TiposIdentificacion;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class TiposIdentificacionController : Controller
    {
        private readonly ITiposIdentificacionApiService _tiposIdentificacionApiService;
        private readonly PaginacionOptions _paginacionOptions;

        public TiposIdentificacionController(
            ITiposIdentificacionApiService tiposIdentificacionApiService,
            IOptions<PaginacionOptions> paginacionOptions)
        {
            _tiposIdentificacionApiService = tiposIdentificacionApiService;
            _paginacionOptions = paginacionOptions.Value;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var todos = await _tiposIdentificacionApiService.GetAllAsync();
            var tamanoPagina = _paginacionOptions.TamanoPagina;
            if (page < 1) page = 1;

            var resultado = new PagedResult<TipoIdentificacionDto>
            {
                PageNumber = page,
                PageSize = tamanoPagina,
                TotalItems = todos.Count,
                Items = todos
                    .OrderBy(t => t.Nombre)
                    .Skip((page - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList()
            };

            return View(resultado);
        }

        public IActionResult Create()
        {
            return View(new TipoIdentificacionFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoIdentificacionFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _tiposIdentificacionApiService.CreateAsync(model.Nombre.Trim());
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                return View(model);
            }

            TempData["Success"] = $"El tipo de identificación '{resultado.Data!.Nombre}' se creó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tipo = await _tiposIdentificacionApiService.GetByIdAsync(id);
            if (tipo is null)
                return NotFound();

            var model = new TipoIdentificacionFormViewModel { Id = tipo.Id, Nombre = tipo.Nombre };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoIdentificacionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Id = id;
                return View(model);
            }

            var resultado = await _tiposIdentificacionApiService.UpdateAsync(id, model.Nombre.Trim());
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                model.Id = id;
                return View(model);
            }

            TempData["Success"] = $"El tipo de identificación '{resultado.Data!.Nombre}' se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _tiposIdentificacionApiService.DeleteAsync(id);
            TempData[eliminado ? "Success" : "Error"] = eliminado
                ? "El tipo de identificación se eliminó correctamente."
                : "No se pudo eliminar el tipo de identificación seleccionado.";

            return RedirectToAction(nameof(Index));
        }
    }
}
