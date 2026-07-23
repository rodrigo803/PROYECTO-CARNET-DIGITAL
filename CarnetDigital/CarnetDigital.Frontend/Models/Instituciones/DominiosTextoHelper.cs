namespace CarnetDigital.Frontend.Models.Instituciones
{
    // Parsea el textarea de dominios: uno por línea, sin vacíos, sin duplicados (case-insensitive).
    public static class DominiosTextoHelper
    {
        public static List<string> Parse(string? texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return new List<string>();

            return texto
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(linea => linea.Trim())
                .Where(linea => linea.Length > 0)
                .GroupBy(linea => linea, StringComparer.OrdinalIgnoreCase)
                .Select(grupo => grupo.First())
                .ToList();
        }
    }
}
