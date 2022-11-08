namespace DDStudy2022.Api.Models.Attachments
{
    public class AttachmentModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string FilePath { get; set; } = null!;

    }
}
