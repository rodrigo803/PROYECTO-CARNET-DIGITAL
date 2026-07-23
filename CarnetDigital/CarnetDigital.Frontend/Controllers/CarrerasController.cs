using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Models.Carreras;
using CarnetDigital.Frontend.Models.Shared;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.Carreras;
using CarnetDigital.Frontend.Services.Instituciones;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class CarrerasController : Controller
    {
        private readonly ICarrerasApiService _carrerasApiService;
        private readonly IInstitucionesApiService _institucionesApiService;
        private readonly PaginacionOptions _paginacionOptions;

        public CarrerasController(
            ICarrerasApiService carrerasApiService,
            IInstitucionesApiService institucionesApiService,
            IOptions<PaginacionOptions> paginacionOptions)
        {
            _carrerasApiService = carrerasApiService;
            _institucionesApiService = institucionesApiService;
            _paginacionOptions = paginacionOptions.Value;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var todas = await _carrerasApiService.GetAllAsync();
            var tamanoPagina = _paginacionOptions.TamanoPagina;
            if (page < 1) page = 1;

            var resultado = new PagedResult<CarreraDto>
            {
                PageNumber = page,
                PageSize = tamanoPagina,
                TotalItems = todas.Count,
                Items = todas
                    .OrderBy(c => c.Nombre)
                    .Skip((page - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList()
            };

            return View(resultado);
        }

        public async Task<IActionResult> Create()
        {
            var model = new CarreraFormViewModel
            {
                Instituciones = await ObtenerInstitucionesSelectListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarreraFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Instituciones = await ObtenerInstitucionesSelectListAsync(model.IdInstitucion);
                return View(model);
            }

            var resultado = await _carrerasApiService.CreateAsync(
                model.Nombre.Trim(), model.Director.Trim(), model.Email.Trim(), model.Telefono.Trim(), model.IdInstitucion!.Value);

            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                model.Instituciones = await ObtenerInstitucionesSelectListAsync(model.IdInstitucion);
                return View(model);
            }

            TempData["Success"] = $"La carrera '{resultado.Data!.Nombre}' se creó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var carrera = await _carrerasApiService.GetByIdAsync(id);
            if (carrera is null)
                return NotFound();

            var model = new CarreraFormViewModel
            {
                Id = carrera.Id,
                Nombre = carrera.Nombre,
                Director = carrera.Director,
                Email = carrera.Email,
                Telefono = carrera.Telefono,
                IdInstitucion = carrera.IdInstitucion,
                Instituciones = await ObtenerInstitucionesSelectListAsync(carrera.IdInstitucion)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarreraFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Id = id;
                model.Instituciones = await ObtenerInstitucionesSelectListAsync(model.IdInstitucion);
                return View(model);
            }

            var resultado = await _carrerasApiService.UpdateAsync(
                id, model.Nombre.Trim(), model.Director.Trim(), model.Email.Trim(), model.Telefono.Trim(), model.IdInstitucion!.Value);

            if (!resultado.Success)
            {
                ModelState.AddModelError(string.Empty, resultado.ErrorMessage!);
                model.Id = id;
                model.Instituciones = await ObtenerInstitucionesSelectListAsync(model.IdInstitucion);
                return View(model);
            }

            TempData["Success"] = $"La carrera '{resultado.Data!.Nombre}' se actualizó correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _carrerasApiService.DeleteAsync(id);
            TempData[eliminado ? "Success" : "Error"] = eliminado
                ? "La carrera se eliminó correctamente."
                : "No se pudo eliminar la carrera seleccionada.";

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
