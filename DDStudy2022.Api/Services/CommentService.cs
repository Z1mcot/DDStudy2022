﻿using AutoMapper;
using AutoMapper.Configuration.Annotations;
using DDStudy2022.Api.Models.Comments;
using DDStudy2022.Api.Models.Likes;
using DDStudy2022.Common.Enums;
using DDStudy2022.Common.Exceptions;
using DDStudy2022.Common.Extensions;
using DDStudy2022.DAL;
using DDStudy2022.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DDStudy2022.Api.Services
{
    public class CommentService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public CommentService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddComment(Guid userId, Guid postId, AddCommentModel model)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new PostNotFoundException();
            if (!await IsAuthorizedToSeeComments(userId, post.AuthorId))
                throw new PrivateAccountException();

            var dbModel = _mapper.Map<PostComment>(model);

            await _context.PostComments.AddAsync(dbModel);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveComment(Guid userId, Guid commentId)
        {
            var comment = await GetCommentById(commentId);
            if (userId != comment.AuthorId)
                throw new ModifyCommentException();

            _context.PostComments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task ModifyComment(ModifyCommentModel model, Guid userId, Guid commentId)
        {
            var comment = await GetCommentById(commentId);
            if (userId != comment.AuthorId)
                throw new ModifyCommentException();

            if (model.Content != null && model.Content != comment.Content)
            {
                comment.Content = model.Content;
                comment.IsModified = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<CommentModel>> GetPostComments(Guid userId, Guid postId, int skip, int take)
        {
            var post = await _context.Posts.Include(c => c.Likes).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null)
                throw new PostNotFoundException();
            if (!await IsAuthorizedToSeeComments(userId, post.AuthorId))
                throw new PrivateAccountException();
            
            var dbComments = await _context.PostComments
                .Include(s => s.Author).ThenInclude(s => s.Avatar)
                .AsNoTracking()
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.PublishDate).Skip(skip).Take(take)
                .ToListAsync();

            var comments = new List<CommentModel>();
            foreach (var comment in dbComments)
            {
                var model = _mapper.Map<PostComment, CommentModel>(comment, opt =>
                {
                    opt.AfterMap((src, dest) 
                        => dest.IsLiked = src.Likes != null && src.Likes.Any(l => l.UserId == userId && l.CommentId == comment.Id) ? 1 : 0);
                });
                comments.Add(model);
            }

            return comments;
        }

        public async Task LikeComment(Guid userId, Guid commentId)
        {
            var comment = await _context.PostComments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == commentId);

            if (comment == null)
                throw new CommentNotFoundException();
            
            if (!await IsAuthorizedToSeeComments(userId!, comment.AuthorId))
                throw new PrivateAccountException();

            var like = await _context.CommentLikes.FirstOrDefaultAsync(x => x.UserId == userId && x.CommentId == commentId);
            if (like != null)
            {
                _context.CommentLikes.Remove(like);
            } 
            else
            {
                var likeModel = new CommentLike
                {
                    CommentId = commentId,
                    UserId = userId
                };
                var dbLike = _mapper.Map<CommentLike>(likeModel);
                await _context.CommentLikes.AddAsync(dbLike);
            }
               
            await _context.SaveChangesAsync();
        }

        private async Task<PostComment> GetCommentById(Guid commentId)
        {
            var comment = await _context.PostComments.FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
                throw new CommentNotFoundException();
            
            return comment;
        }

        private async Task<bool> IsAuthorizedToSeeComments(Guid userId, Guid authorId)
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
