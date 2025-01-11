namespace DocumentRegisteryAppApi.Models
{
    public class UpdateDocumentModel
    {
        public Guid Id { get; set; }
        public string? Number { get; set; }
        public string? OutNumber { get; set; }
        public DateOnly? OutDate { get; set; }
        public string? DeliveryMethod { get; set; }
        public string? Correspondent { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public DateOnly? DueDate { get; set; }
        public bool? Access { get; set; } = false;
        public bool? Control { get; set; } = false;
        public IFormFile? File { get; set; }
    }
}
