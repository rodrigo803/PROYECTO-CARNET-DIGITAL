using CarnetDigital.Frontend.Filters;
using CarnetDigital.Frontend.Options;
using CarnetDigital.Frontend.Services.Bitacoras;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CarnetDigital.Frontend.Controllers
{
    [RequiereSesion]
    public class BitacorasController : Controller
    {
        private readonly IBitacorasApiService _bitacorasApiService;
        private readonly PaginacionOptions _paginacionOptions;

        public BitacorasController(
            IBitacorasApiService bitacorasApiService,
            IOptions<PaginacionOptions> paginacionOptions)
        {
            _bitacorasApiService = bitacorasApiService;
            _paginacionOptions = paginacionOptions.Value;
        }

        public async Task<IActionResult> Index(DateTime? fecha, int? usuarioId, string? descripcion, int page = 1)
        {
            if (page < 1) page = 1;
            var tamanoPagina = _paginacionOptions.TamanoPagina;

            var resultado = await _bitacorasApiService.GetPagedAsync(
                fecha, usuarioId, descripcion, page, tamanoPagina);

            ViewBag.Fecha = fecha?.ToString("yyyy-MM-dd");
            ViewBag.UsuarioId = usuarioId;
            ViewBag.Descripcion = descripcion;

            return View(resultado);
        }
    }
}
