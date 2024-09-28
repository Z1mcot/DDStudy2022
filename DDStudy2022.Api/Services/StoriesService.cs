using AutoMapper;
using DDStudy2022.Api.Models.Attachments;
using DDStudy2022.Api.Models.Stories;
using DDStudy2022.Common.Enums;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDStudy2022.Api.Services
{
    public class StoriesService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        public StoriesService(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task AddStoriesToUser(CreateStoriesRequest request)
        {
            var model = _mapper.Map<CreateStoriesModel>(request);

            model.Content.AuthorId = model.AuthorId;
            model.Content.FilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Attachments",
                    model.Content.TempId.ToString());

            var tempFi = new FileInfo(Path.Combine(Path.GetTempPath(), model.Content.TempId.ToString()));
            if (tempFi.Exists)
            {
                var destFi = new FileInfo(model.Content.FilePath);
                if (destFi.Directory is { Exists: false })
                    destFi.Directory.Create();

                File.Move(tempFi.FullName, model.Content.FilePath, true);
            }

            var dbStories = _mapper.Map<Stories>(model);

            await _context.Stories.AddAsync(dbStories);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveStory(Guid userId, Guid storyId)
        {
            var story = await _context.Stories.FirstOrDefaultAsync(s => s.IsShown && s.Id == storyId);
            if (story == null)
                throw new StoriesNotFoundException();
            if (story.AuthorId != userId)
                throw new DeleteStoryException();

            story.IsShown = false;
            await _context.SaveChangesAsync();
        }

        public async Task<List<StoriesModel>> GetStoriesFromSubscriptions(Guid userId)
        {
            var dtNow = DateTime.UtcNow;
            var stories = await _context.Stories
                .Include(s => s.Author).ThenInclude(s => s.Avatar)
                .Include(s => s.Author).ThenInclude(s => s.Subscribers)
                .Include(s => s.Content)
                .Where(s => s.AuthorId != userId && s.IsShown && s.ExpirationDate > dtNow 
                            && s.Author.IsActive
                            && (!s.Author.IsPrivate || s.Author.Subscribers!.Any(u => u.SubscriberId == userId && u.Status == SubscriptionStatus.Active)))
                .Select(s => _mapper.Map<StoriesModel>(s))
                .ToListAsync();

            return stories;
        }

        public async Task<List<StoriesModel>> GetUserStories(Guid userId, Guid authorId)
        {
            if (!await IsAuthorizedToSeeStories(userId, authorId))
                throw new PrivateAccountException();

            var dtNow = DateTime.UtcNow;
            var stories = await _context.Stories
                .Include(s => s.Author).ThenInclude(s => s.Avatar)
                .Include(s => s.Content)
                .Where(s => s.IsShown && s.AuthorId == authorId && s.ExpirationDate > dtNow)
                .Select(s => _mapper.Map<StoriesModel>(s))
                .ToListAsync();

            return stories;
        }

        public async Task<StoriesModel> GetStoryById(Guid userId, Guid storyId)
        {
            var dtNow = DateTime.UtcNow;
            var story = await _context.Stories
                .Include(s => s.Author).ThenInclude(s => s.Avatar)
                .Include(s => s.Content)
                .FirstOrDefaultAsync(s => s.IsShown && s.Id == storyId && s.ExpirationDate > dtNow);

            if (story == null)
                throw new StoriesNotFoundException();
            if (!await IsAuthorizedToSeeStories(userId, story.AuthorId))
                throw new PrivateAccountException();

            return _mapper.Map<StoriesModel>(story);
        }

        public async Task<AttachmentModel> GetStoryContent(Guid userId, Guid storyContentId)
        {
            var dtNow = DateTime.UtcNow;
            var res = await _context.StoriesContent
                .Include(x => x.Stories)
                .FirstOrDefaultAsync(x => x.Id == storyContentId && x.Stories.IsShown && x.Stories.ExpirationDate > dtNow);
            
            if (res == null)
                throw new AttachmentNotFoundException();
            if (!await IsAuthorizedToSeeStories(userId, res.AuthorId))
                throw new PrivateAccountException();

            return _mapper.Map<AttachmentModel>(res);
        }

        private async Task<bool> IsAuthorizedToSeeStories(Guid userId, Guid authorId)
        {
            if (userId == authorId)
                return true;

            var dbAuthor = await _context.Users.Include(u => u.Subscribers).FirstAsync(u => u.Id == authorId);
            if (!dbAuthor.IsActive)
                return false;

            return !dbAuthor.IsPrivate || dbAuthor.Subscribers!.Any(s => s.SubscriberId == userId && s.Status == SubscriptionStatus.Active);
        }
    }
}
