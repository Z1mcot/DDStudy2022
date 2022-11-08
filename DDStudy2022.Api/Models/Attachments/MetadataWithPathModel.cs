namespace DDStudy2022.Api.Models.Attachments
{
    public class MetadataWithPathModel : MetadataModel
    {
        public string FilePath { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public MetadataWithPathModel(MetadataModel model, Func<MetadataModel, string> pathgen, Guid authorId)
        {
            TempId = model.TempId;
            Name = model.Name;
            MimeType = model.MimeType;
            Size = model.Size;
            FilePath = pathgen(model);
            AuthorId = authorId;
        }
    }
}
