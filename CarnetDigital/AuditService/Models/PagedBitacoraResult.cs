namespace AuditService.Models
{
    public class PagedBitacoraResult
    {
        public List<BitacoraDto> Items { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
