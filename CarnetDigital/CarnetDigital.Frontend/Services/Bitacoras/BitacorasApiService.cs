using System.Net.Http.Json;
using CarnetDigital.Frontend.Models.Bitacoras;
using CarnetDigital.Frontend.Models.Shared;
using Microsoft.AspNetCore.WebUtilities;

namespace CarnetDigital.Frontend.Services.Bitacoras
{
    public class BitacorasApiService : IBitacorasApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BitacorasApiService> _logger;

        public BitacorasApiService(HttpClient httpClient, ILogger<BitacorasApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PagedResult<BitacoraDto>> GetPagedAsync(
            DateTime? fecha,
            int? usuarioId,
            string? descripcion,
            int page,
            int pageSize)
        {
            var query = new Dictionary<string, string?>
            {
                ["page"] = page.ToString(),
                ["pageSize"] = pageSize.ToString()
            };

            if (fecha.HasValue)
                query["fecha"] = fecha.Value.ToString("yyyy-MM-dd");

            if (usuarioId.HasValue)
                query["usuarioId"] = usuarioId.Value.ToString();

            if (!string.IsNullOrWhiteSpace(descripcion))
                query["descripcion"] = descripcion;

            var url = QueryHelpers.AddQueryString("/bitacora", query);

            try
            {
                var resultado = await _httpClient.GetFromJsonAsync<PagedBitacoraResponse>(url);

                if (resultado is null)
                    return new PagedResult<BitacoraDto> { PageNumber = page, PageSize = pageSize };

                return new PagedResult<BitacoraDto>
                {
                    Items = resultado.Items,
                    PageNumber = resultado.Page,
                    PageSize = resultado.PageSize,
                    TotalItems = resultado.Total
                };
            }
            catch (Exception ex) when (ex is HttpRequestException or NotSupportedException or System.Text.Json.JsonException)
            {
                _logger.LogError(ex, "Error al obtener bitácoras");
                return new PagedResult<BitacoraDto> { PageNumber = page, PageSize = pageSize };
            }
        }

        private class PagedBitacoraResponse
        {
            public List<BitacoraDto> Items { get; set; } = new();
            public int Total { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }
    }
}
