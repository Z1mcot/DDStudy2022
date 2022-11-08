using AutoMapper.Configuration.Annotations;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class AttachmentController : ControllerBase
    {
        private readonly AttachmentService _attachmentService;
        

        public AttachmentController(AttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        [HttpPost]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
            => await _attachmentService.UploadFiles(files);

    }
}
