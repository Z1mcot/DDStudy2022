namespace DDStudy2022.Api.Models
{
    public class MetadataModel
    {
        public Guid TempId { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; }
    }
}
