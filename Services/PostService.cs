using api.Data;
using api.Dto;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;
public class PostService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MinioService _minioService;

    public PostService(ApplicationDbContext dbContext, MinioService minioService)
    {
        _dbContext = dbContext;
        _minioService = minioService;
    }

    public async Task<PaginatedResultDto<PostSummaryDto>> GetPostsAsync(int page, int pageSize, string? sortDirection, List<string>? hashtags, string? searchTitle)
    {
        var query = _dbContext.Posts.AsQueryable();

        if (hashtags != null && hashtags.Count > 0)
        {
            query = query.Where(p => hashtags.All(tag => p.Hashtags.Contains(tag)));
        }

        if (!string.IsNullOrEmpty(searchTitle))
        {
            var lowerSearchName = searchTitle.ToLower();
            query = query.Where(p => EF.Functions.Like(p.Title.ToLower(), $"%{lowerSearchName}%"));
        }

        query = sortDirection?.ToLower() switch
        {
            "asc" => query.OrderBy(p => p.CreatedAt),
            "desc" => query.OrderByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync();

         var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostSummaryDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body.Length > 100 ? p.Body.Substring(0, 100) + "..." : p.Body,
                Image = p.Image
            })
            .ToListAsync();

        return new PaginatedResultDto<PostSummaryDto>
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        return await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Post> CreatePostAsync(PostDto post)
    {
        var newPost = new Post
        {
            Id = Guid.NewGuid(),
            Title = post.Title,
            Body = post.Body,
            Hashtags = post.Hashtags,
        };

        if (post.Image != null) 
        {

            using var stream = post.Image.OpenReadStream();
            var objectName = await _minioService.UploadFileAsync(post.Image.FileName, stream);
            newPost.Image = _minioService.GetFileUrl(objectName);
        }

        _dbContext.Posts.Add(newPost);
        await _dbContext.SaveChangesAsync();
        return newPost;
    }

    public async Task<Post?> UpdatePostAsync(Guid id, PostDto updatedPost)
    {
        var existingPost = await _dbContext.Posts.FindAsync(id);
        if (existingPost == null) return null;

        existingPost.Title = updatedPost.Title;
        existingPost.Body = updatedPost.Body;
        existingPost.Hashtags = updatedPost.Hashtags;

        if (updatedPost.Image != null) 
        {
            if (!string.IsNullOrEmpty(existingPost.Image))
            {
                var imageName = existingPost.Image.Split('/').Last();
                await _minioService.DeleteFileAsync(imageName);
            }

            using var stream = updatedPost.Image.OpenReadStream();
            var objectName = await _minioService.UploadFileAsync(updatedPost.Image.FileName, stream);
            existingPost.Image = _minioService.GetFileUrl(objectName);
        }

        await _dbContext.SaveChangesAsync();
        return existingPost;
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var post = await _dbContext.Posts.FindAsync(id);
        if (post == null) return false;

        if (!string.IsNullOrEmpty(post.Image))
        {
            var objectName = post.Image.Split('/').Last();
            await _minioService.DeleteFileAsync(objectName);
        }


        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<Post> AddLikeDislikeToPostAsync(Guid postId, string action)
    {
        var post = await _dbContext.Posts.FindAsync(postId);
        if (post == null)
        {
            throw new InvalidOperationException("Post not found.");
        }

        if (action == "Like")
        {
            post.Likes++;
        }
        else if (action == "Dislike")
        {
            post.Dislikes++;
        }

        await _dbContext.SaveChangesAsync();
        return post;
    }
}
