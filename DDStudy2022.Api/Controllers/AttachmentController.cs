using AutoMapper.Configuration.Annotations;
using DDStudy2022.Api.Models;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Api.Models.Posts;
using DDStudy2022.Api.Services;
using DDStudy2022.Common.Consts;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DDStudy2022.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class AttachmentController : ControllerBase
    {
        private readonly AttachmentService _attachmentService;
        private readonly PostService _postService;
        private readonly UserService _userService;
        public AttachmentController(AttachmentService attachmentService, PostService postService, UserService userService)
        {
            _attachmentService = attachmentService;
            _postService = postService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<List<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
            => await _attachmentService.UploadFiles(files);

        [HttpGet]
        [AllowAnonymous]
        [Route("{postContentId}")]
        public async Task<FileStreamResult> GetPostContent(Guid postContentId, bool download = false)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            //if (userId == default)
            //    throw new IdClaimConversionException();
            
            return RenderAttachment(await _postService.GetPostContent(userId, postContentId), download);
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<FileStreamResult> GetUserAvatar(Guid userId, bool download = false)
            => RenderAttachment(await _userService.GetUserAvatar(userId), download);

        [HttpGet]
        public async Task<FileStreamResult> GetCurrentUserAvatar(bool download = false)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId == default)
                throw new IdClaimConversionException();

            return await GetUserAvatar(userId, download);

        }

        [HttpPost]
        public async Task AddAvatarToUser(MetadataModel model)
        {
            var userId = User.GetClaimValue<Guid>(ClaimNames.Id);
            if (userId != default)
            {
                var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.TempId.ToString()));
                if (!tempFi.Exists)
                    throw new TempFileNotFoundException();
                else
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", model.TempId.ToString());
                    var destFi = new FileInfo(path);
                    if (destFi.Directory != null && !destFi.Directory.Exists)
                        destFi.Directory.Create();

                    System.IO.File.Copy(tempFi.FullName, path, true);

                    await _userService.AddAvatarToUser(userId, model, path);
                }
            }
            else
                throw new IdClaimConversionException();
        }


        private FileStreamResult RenderAttachment(AttachmentModel attach, bool download)
        {
            var fs = new FileStream(attach.FilePath, FileMode.Open);
            var ext = Path.GetExtension(attach.Name);
            if (download)
                return File(fs, attach.MimeType, $"{attach.Id}{ext}");
            else
                return File(fs, attach.MimeType);
        }
    }
}
