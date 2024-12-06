using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;
public class PostService
{
    private readonly ApplicationDbContext _dbContext;

    public PostService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Post>> GetPostsAsync(int page, int pageSize, string? sortDirection, List<string>? hashtags, string? searchTitle)
    {
        var query = _dbContext.Posts.Include(p => p.User).AsQueryable();

        if (hashtags != null && hashtags.Count > 0)
        {
            query = query.Where(p => hashtags.All(tag => p.Hashtags.Contains(tag)));
        }

        if (!string.IsNullOrEmpty(searchTitle))
        {
            query = query.Where(p => EF.Functions.Like(p.Title, $"%{searchTitle}%"));
        }

        query = sortDirection?.ToLower() switch
        {
            "asc" => query.OrderBy(p => p.CreatedAt),
            "desc" => query.OrderByDescending(p => p.CreatedAt),
            _ => query
        };

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Post?> GetPostByIdAsync(Guid id)
    {
        return await _dbContext.Posts.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
    }


    public async Task<Post> CreatePostAsync(Post post)
    {
        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync();
        return post;
    }

    public async Task<Post?> UpdatePostAsync(Guid id, Post updatedPost)
    {
        var existingPost = await _dbContext.Posts.FindAsync(id);
        if (existingPost == null) return null;

        existingPost.Title = updatedPost.Title;
        existingPost.Body = updatedPost.Body;
        existingPost.Hashtags = updatedPost.Hashtags;
        await _dbContext.SaveChangesAsync();
        return existingPost;
    }

    public async Task<bool> DeletePostAsync(Guid id)
    {
        var post = await _dbContext.Posts.FindAsync(id);
        if (post == null) return false;

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
