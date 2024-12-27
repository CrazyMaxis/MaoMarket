using api.Data;
using api.Dto;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class CommentService
{
    private readonly ApplicationDbContext _dbContext;

    public CommentService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<CommentResponseDto>> GetCommentsByPostIdAsync(Guid postId)
    {
        return await _dbContext.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .Select(c => new CommentResponseDto
            {
                Id = c.Id,
                Body = c.Body,
                CreatedAt = c.CreatedAt,
                User = new UserNameDto
                {
                    Id = c.User!.Id,
                    Name = c.User.Name
                }
            })
            .ToListAsync();
    }


    public async Task<Comment?> GetCommentByIdAsync(Guid id)
    {
        return await _dbContext.Comments.FindAsync(id);
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();
        return comment;
    }

    public async Task<Comment?> UpdateCommentAsync(Guid id, string newBody)
    {
        var comment = await _dbContext.Comments.FindAsync(id);
        if (comment == null) return null;

        comment.Body = newBody;
        await _dbContext.SaveChangesAsync();
        return comment;
    }

    public async Task<bool> DeleteCommentAsync(Guid id)
    {
        var comment = await _dbContext.Comments.FindAsync(id);
        if (comment == null) return false;

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
