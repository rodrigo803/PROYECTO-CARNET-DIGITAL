using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Microservicio.Usuario.Services
{
    // 1. La interfaz vive a nivel de namespace
    public interface ICatalogosApiClient
    {
        Task ValidarCatalogosAsync(int tipoId, int tipoUsr, List<int> instIds, List<int> carrerasIds, List<int> areasIds, string? token);
        Task<string> ObtenerNombreTipoUsuarioAsync(int id, string? token);
        Task<string> ObtenerNombresInstitucionesAsync(List<int> ids, string? token);
        Task<string> ObtenerNombresCarrerasOAreasAsync(int tipoUsuarioId, List<int> carrerasIds, List<int> areasIds, string? token);
    }

    // 2. La clase vive a nivel de namespace (ya no está anidada)
    public class CatalogosApiClient : ICatalogosApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CatalogosApiClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        private async Task<(bool Existe, string Nombre)> ConsultarMicroservicioAsync(string nombreMicroservicio, string endpoint, string? token)
        {
            string baseUrl = _config[$"Microservicios:{nombreMicroservicio}"];
            if (string.IsNullOrEmpty(baseUrl)) return (false, "");

            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"{baseUrl}{endpoint}");
                if (!response.IsSuccessStatusCode) return (false, "");

                var json = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(json);

                if (doc.RootElement.TryGetProperty("nombre", out var val1)) return (true, val1.GetString());
                if (doc.RootElement.TryGetProperty("Nombre", out var val2)) return (true, val2.GetString());

                return (true, "Desconocido");
            }
            catch
            {
                return (false, "");
            }
        }

        public async Task ValidarCatalogosAsync(int tipoId, int tipoUsr, List<int> instIds, List<int> carrerasIds, List<int> areasIds, string? token)
        {
            var resTipoId = await ConsultarMicroservicioAsync("TiposIdentificacion", $"/api/tiposidentificacion/{tipoId}", token);
            if (!resTipoId.Existe) throw new Exception("El Tipo de Identificación no existe.");

            var resTipoUsr = await ConsultarMicroservicioAsync("TiposUsuarios", $"/api/TiposUsuario/{tipoUsr}", token);
            if (!resTipoUsr.Existe) throw new Exception("El Tipo de Usuario no existe.");

            if (instIds != null)
            {
                foreach (var inst in instIds)
                {
                    var resInst = await ConsultarMicroservicioAsync("Instituciones", $"/api/instituciones/{inst}", token);
                    if (!resInst.Existe) throw new Exception($"La institución ID {inst} no existe.");
                }
            }

            bool esEstudiante = string.Equals(resTipoUsr.Nombre, "Estudiante", StringComparison.OrdinalIgnoreCase);
            bool esFuncionario = string.Equals(resTipoUsr.Nombre, "Funcionario", StringComparison.OrdinalIgnoreCase);

            if (esEstudiante && carrerasIds != null)
            {
                foreach (var carrera in carrerasIds)
                {
                    var resCarrera = await ConsultarMicroservicioAsync("Carreras", $"/api/carreras/{carrera}", token);
                    if (!resCarrera.Existe) throw new Exception($"La carrera ID {carrera} no existe.");
                }
            }

            if (esFuncionario && areasIds != null)
            {
                foreach (var area in areasIds)
                {
                    var resArea = await ConsultarMicroservicioAsync("Areas", $"/api/areas/{area}", token);
                    if (!resArea.Existe) throw new Exception($"El área ID {area} no existe.");
                }
            }
        }

        public async Task<string> ObtenerNombreTipoUsuarioAsync(int id, string? token)
        {
            var res = await ConsultarMicroservicioAsync("TiposUsuarios", $"/api/TiposUsuario/{id}", token);
            return res.Existe ? res.Nombre : "Desconocido";
        }

        public async Task<string> ObtenerNombresInstitucionesAsync(List<int> ids, string? token)
        {
            if (ids == null || ids.Count == 0) return "Ninguna";
            var nombres = new List<string>();
            foreach (var id in ids)
            {
                var res = await ConsultarMicroservicioAsync("Instituciones", $"/api/instituciones/{id}", token);
                if (res.Existe) nombres.Add(res.Nombre);
            }
            return string.Join(", ", nombres);
        }

        public async Task<string> ObtenerNombresCarrerasOAreasAsync(int tipoUsr, List<int> carreras, List<int> areas, string? token)
        {
            var nombreTipoUsr = await ObtenerNombreTipoUsuarioAsync(tipoUsr, token);
            bool esEstudiante = string.Equals(nombreTipoUsr, "Estudiante", StringComparison.OrdinalIgnoreCase);
            bool esFuncionario = string.Equals(nombreTipoUsr, "Funcionario", StringComparison.OrdinalIgnoreCase);

            var nombres = new List<string>();
            if (esEstudiante && carreras != null)
            {
                foreach (var id in carreras)
                {
                    var res = await ConsultarMicroservicioAsync("Carreras", $"/api/carreras/{id}", token);
                    if (res.Existe) nombres.Add(res.Nombre);
                }
            }
            else if (esFuncionario && areas != null)
            {
                foreach (var id in areas)
                {
                    var res = await ConsultarMicroservicioAsync("Areas", $"/api/areas/{id}", token);
                    if (res.Existe) nombres.Add(res.Nombre);
                }
            }
            return nombres.Count > 0 ? string.Join(", ", nombres) : "Ninguna asignada";
        }
    }
}