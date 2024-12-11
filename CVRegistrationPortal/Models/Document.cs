namespace CVRegistrationPortal.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Path { get; set; }
        public string FileExtension { get; set; }
        public int UserID { get; set; }
        public DateTime UploadedOn { get; set; } = DateTime.Now.Date;
        
    }
}
