using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models.Shared;
using CarnetDigital.Frontend.Models.TiposUsuario;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.TiposUsuario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class TiposUsuarioController : Controller
    {
        private readonly ITiposUsuarioApiService _tiposUsuarioApiService;
        private readonly PaginacionOptions _paginacionOptions;

        public TiposUsuarioController(
            ITiposUsuarioApiService tiposUsuarioApiService,
            IOptions<PaginacionOptions> paginacionOptions)
        {
            _tiposUsuarioApiService = tiposUsuarioApiService;
            _paginacionOptions = paginacionOptions.Value;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var todos = await _tiposUsuarioApiService.GetAllAsync();
            var tamanoPagina = _paginacionOptions.TamanoPagina;
            if (page < 1) page = 1;

            var resultado = new PagedResult<TipoUsuarioDto>
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
            return View(new TipoUsuarioFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoUsuarioFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _tiposUsuarioApiService.CreateAsync(model.Nombre.Trim());
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                return View(model);
            }

            TempData["Success"] = $"El tipo de usuario '{resultado.Data!.Nombre}' se creó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tipo = await _tiposUsuarioApiService.GetByIdAsync(id);
            if (tipo is null)
                return NotFound();

            var model = new TipoUsuarioFormViewModel { Id = tipo.Id, Nombre = tipo.Nombre };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoUsuarioFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Id = id;
                return View(model);
            }

            var resultado = await _tiposUsuarioApiService.UpdateAsync(id, model.Nombre.Trim());
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                model.Id = id;
                return View(model);
            }

            TempData["Success"] = $"El tipo de usuario '{resultado.Data!.Nombre}' se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _tiposUsuarioApiService.DeleteAsync(id);
            TempData[eliminado ? "Success" : "Error"] = eliminado
                ? "El tipo de usuario se eliminó correctamente."
                : "No se pudo eliminar el tipo de usuario seleccionado.";

            return RedirectToAction(nameof(Index));
        }
    }
}
