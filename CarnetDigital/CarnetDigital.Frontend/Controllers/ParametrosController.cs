using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models.Parametros;
using CarnetDigital.Frontend.Models.Shared;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.Parametros;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class ParametrosController : Controller
    {
        private readonly IParametrosApiService _parametrosApiService;
        private readonly PaginacionOptions _paginacionOptions;

        public ParametrosController(
            IParametrosApiService parametrosApiService,
            IOptions<PaginacionOptions> paginacionOptions)
        {
            _parametrosApiService = parametrosApiService;
            _paginacionOptions = paginacionOptions.Value;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var todos = await _parametrosApiService.GetAllAsync();
            var tamanoPagina = _paginacionOptions.TamanoPagina;
            if (page < 1) page = 1;

            var resultado = new PagedResult<ParametroDto>
            {
                PageNumber = page,
                PageSize = tamanoPagina,
                TotalItems = todos.Count,
                Items = todos
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList()
            };

            return View(resultado);
        }

        public IActionResult Create()
        {
            var model = new ParametroFormViewModel { EsEdicion = false };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ParametroFormViewModel model)
        {
            model.EsEdicion = false;

            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _parametrosApiService.CreateAsync(model.Id.Trim(), model.Valor.Trim());
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                return View(model);
            }

            TempData["Success"] = $"El parámetro '{resultado.Data!.Id}' se creó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var parametro = await _parametrosApiService.GetByIdAsync(id);
            if (parametro is null)
                return NotFound();

            var model = new ParametroFormViewModel
            {
                Id = parametro.Id,
                Valor = parametro.Valor,
                EsEdicion = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ParametroFormViewModel model)
        {
            model.Id = id;
            model.EsEdicion = true;

            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _parametrosApiService.UpdateAsync(id, model.Valor.Trim());
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                return View(model);
            }

            TempData["Success"] = $"El parámetro '{id}' se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var eliminado = await _parametrosApiService.DeleteAsync(id);
            TempData[eliminado ? "Success" : "Error"] = eliminado
                ? "El parámetro se eliminó correctamente."
                : "No se pudo eliminar el parámetro seleccionado.";

            return RedirectToAction(nameof(Index));
        }
    }
}
