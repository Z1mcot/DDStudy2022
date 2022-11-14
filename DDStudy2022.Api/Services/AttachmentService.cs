using AutoMapper;
using DDStudy2022.Api.Configs;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DDStudy2022.Api.Services
{
    public class AttachmentService
    {

        public AttachmentService()
        {
            
        }

        public async Task<MetadataModel> UploadFile(IFormFile file)
        {
            var tempPath = Path.GetTempPath();
            var meta = new MetadataModel
            {
                TempId = Guid.NewGuid(),
                Name = file.FileName,
                MimeType = file.ContentType,
                Size = file.Length
            };

            var newPath = Path.Combine(tempPath, meta.TempId.ToString());
            var fileInfo = new FileInfo(newPath);

            if (fileInfo.Exists)
                throw new Exception("Temp file with this name already exists");

            if (fileInfo.Directory == null)
                throw new Exception("temp file directory is null");
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory?.Create();

            using (var stream = File.Create(newPath))
            {
                await file.CopyToAsync(stream);
            }

            return meta;
        }

        public async Task<List<MetadataModel>> UploadFiles(List<IFormFile> files)
        {
            var res = new List<MetadataModel>();
            foreach (var file in files)
            {
                res.Add(await UploadFile(file));
            }
            return res;
        }



    }
}
