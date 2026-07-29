using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models.Roles;
using CarnetDigital.Frontend.Models.Shared;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.Pantallas;
using CarnetDigital.Frontend.Services.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class RolesController : Controller
    {
        private readonly IRolesApiService _rolesApiService;
        private readonly IPantallasApiService _pantallasApiService;
        private readonly PaginacionOptions _paginacionOptions;

        public RolesController(
            IRolesApiService rolesApiService,
            IPantallasApiService pantallasApiService,
            IOptions<PaginacionOptions> paginacionOptions)
        {
            _rolesApiService = rolesApiService;
            _pantallasApiService = pantallasApiService;
            _paginacionOptions = paginacionOptions.Value;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var todos = await _rolesApiService.GetAllAsync();
            var tamanoPagina = _paginacionOptions.TamanoPagina;
            if (page < 1) page = 1;

            var resultado = new PagedResult<RolDto>
            {
                PageNumber = page,
                PageSize = tamanoPagina,
                TotalItems = todos.Count,
                Items = todos
                    .OrderBy(r => r.Id)
                    .Skip((page - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList()
            };

            return View(resultado);
        }

        public async Task<IActionResult> Create()
        {
            var model = new RolFormViewModel
            {
                EsEdicion = false,
                PantallasDisponibles = await ObtenerPantallasCheckboxAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolFormViewModel model)
        {
            model.EsEdicion = false;

            if (!ModelState.IsValid)
            {
                model.PantallasDisponibles = await ObtenerPantallasCheckboxAsync(model.PantallasSeleccionadas);
                return View(model);
            }

            var dto = new RolDto
            {
                Id = model.Id.Trim(),
                Nombre = model.Nombre.Trim(),
                Pantallas = model.PantallasSeleccionadas
            };

            var resultado = await _rolesApiService.CreateAsync(dto);
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                model.PantallasDisponibles = await ObtenerPantallasCheckboxAsync(model.PantallasSeleccionadas);
                return View(model);
            }

            TempData["Success"] = $"El rol '{resultado.Data!.Id}' se creó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var rol = await _rolesApiService.GetByIdAsync(id);
            if (rol is null)
                return NotFound();

            var model = new RolFormViewModel
            {
                Id = rol.Id,
                Nombre = rol.Nombre,
                PantallasSeleccionadas = rol.Pantallas,
                PantallasDisponibles = await ObtenerPantallasCheckboxAsync(rol.Pantallas),
                EsEdicion = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RolFormViewModel model)
        {
            model.Id = id;
            model.EsEdicion = true;

            if (!ModelState.IsValid)
            {
                model.PantallasDisponibles = await ObtenerPantallasCheckboxAsync(model.PantallasSeleccionadas);
                return View(model);
            }

            var dto = new RolDto
            {
                Id = id,
                Nombre = model.Nombre.Trim(),
                Pantallas = model.PantallasSeleccionadas
            };

            var resultado = await _rolesApiService.UpdateAsync(id, dto);
            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                model.PantallasDisponibles = await ObtenerPantallasCheckboxAsync(model.PantallasSeleccionadas);
                return View(model);
            }

            TempData["Success"] = $"El rol '{id}' se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var eliminado = await _rolesApiService.DeleteAsync(id);
            TempData[eliminado ? "Success" : "Error"] = eliminado
                ? "El rol se eliminó correctamente."
                : "No se pudo eliminar el rol seleccionado.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<PantallaCheckboxItem>> ObtenerPantallasCheckboxAsync(List<string>? seleccionadas = null)
        {
            var pantallas = await _pantallasApiService.GetAllAsync();
            var seleccionadasSet = (seleccionadas ?? new List<string>()).ToHashSet();

            return pantallas
                .OrderBy(p => p.Nombre)
                .Select(p => new PantallaCheckboxItem
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Seleccionado = seleccionadasSet.Contains(p.Id)
                })
                .ToList();
        }
    }
}
