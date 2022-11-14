namespace DDStudy2022.Api.Models.Attachments
{
    public class AttachmentExternalModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string? ContentLink { get; set; } = null!;

        //public AttachmentExternalModel(AttachmentModel model, Func<AttachmentModel, string?>? linkGenerator)
        //{
        //    Id = model.Id;
        //    Name = model.Name;
        //    MimeType = model.MimeType;
        //    ContentLink = linkGenerator?.Invoke(model);
        //}
    }
}
