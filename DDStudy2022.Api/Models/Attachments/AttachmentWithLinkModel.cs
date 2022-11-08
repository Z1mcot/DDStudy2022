namespace DDStudy2022.Api.Models.Attachments
{
    public class AttachmentWithLinkModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string? ContentLink { get; set; } = null!;

        public AttachmentWithLinkModel(AttachmentModel model, Func<AttachmentModel, string?>? linkGenerator)
        {
            Id = model.Id;
            Name = model.Name;
            MimeType = model.MimeType;
            ContentLink = linkGenerator?.Invoke(model);
        }
    }
}
