using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public bool VerifyPassword(User user, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, user.Password);
    }

    public async Task AddRefreshTokenAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task RemoveRefreshTokenAsync(RefreshToken token)
    {
        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync();
    }

    public async Task CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(User user) 
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
