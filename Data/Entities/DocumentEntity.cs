namespace DocumentRegisteryAppApi.Data.Entities
{
    public class DocumentEntity
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Number { get; set; }
        public string OutNumber { get; set; }
        public DateOnly OutDate { get; set; }
        public string DeliveryMethod { get; set; }
        public string Correspondent { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public DateOnly DueDate { get; set; }
        public bool Access { get; set; } = false;
        public bool Control { get; set; } = false;
        public string FileName { get; set; }
        public DateTime CreatedAt { get; } = DateTime.Now;
    }
}
