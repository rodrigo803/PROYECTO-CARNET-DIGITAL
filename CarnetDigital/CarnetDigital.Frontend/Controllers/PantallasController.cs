using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models.Pantallas;
using CarnetDigital.Frontend.Models.Shared;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.Pantallas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class PantallasController : Controller
    {
        private readonly IPantallasApiService _pantallasApiService;
        private readonly PaginacionOptions _paginacionOptions;

        public PantallasController(
            IPantallasApiService pantallasApiService,
            IOptions<PaginacionOptions> paginacionOptions)
        {
            _pantallasApiService = pantallasApiService;
            _paginacionOptions = paginacionOptions.Value;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var todas = await _pantallasApiService.GetAllAsync();
            var tamanoPagina = _paginacionOptions.TamanoPagina;
            if (page < 1) page = 1;

            var resultado = new PagedResult<PantallaDto>
            {
                PageNumber = page,
                PageSize = tamanoPagina,
                TotalItems = todas.Count,
                Items = todas
                    .OrderBy(p => p.Id)
                    .Skip((page - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList()
            };

            return View(resultado);
        }

        public IActionResult Create()
        {
            var model = new PantallaFormViewModel { EsEdicion = false };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PantallaFormViewModel model)
        {
            model.EsEdicion = false;

            if (!ModelState.IsValid)
                return View(model);

            var dto = new PantallaDto
            {
                Id = model.Id.Trim(),
                Nombre = model.Nombre.Trim(),
                Descripcion = model.Descripcion.Trim(),
                Ruta = model.Ruta.Trim()
            };

            var resultado = await _pantallasApiService.CreateAsync(dto);
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                return View(model);
            }

            TempData["Success"] = $"La pantalla '{resultado.Data!.Id}' se creó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var pantalla = await _pantallasApiService.GetByIdAsync(id);
            if (pantalla is null)
                return NotFound();

            var model = new PantallaFormViewModel
            {
                Id = pantalla.Id,
                Nombre = pantalla.Nombre,
                Descripcion = pantalla.Descripcion,
                Ruta = pantalla.Ruta,
                EsEdicion = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, PantallaFormViewModel model)
        {
            model.Id = id;
            model.EsEdicion = true;

            if (!ModelState.IsValid)
                return View(model);

            var dto = new PantallaDto
            {
                Id = id,
                Nombre = model.Nombre.Trim(),
                Descripcion = model.Descripcion.Trim(),
                Ruta = model.Ruta.Trim()
            };

            var resultado = await _pantallasApiService.UpdateAsync(id, dto);
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                return View(model);
            }

            TempData["Success"] = $"La pantalla '{id}' se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var eliminada = await _pantallasApiService.DeleteAsync(id);
            TempData[eliminada ? "Success" : "Error"] = eliminada
                ? "La pantalla se eliminó correctamente."
                : "No se pudo eliminar la pantalla seleccionada.";

            return RedirectToAction(nameof(Index));
        }
    }
}
