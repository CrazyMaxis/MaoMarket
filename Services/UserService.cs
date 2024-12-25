using api.Data;
using api.Dto;
using api.Models;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResultDto<User>> GetUsersAsync(
        int pageNumber, 
        int pageSize, 
        string? role = null, 
        bool? isBlocked = null, 
        string? searchName = null)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrEmpty(role))
        {
            query = query.Where(u => u.Role == role);
        }

        if (isBlocked.HasValue)
        {
            query = query.Where(u => u.IsBlocked == isBlocked.Value);
        }

        if (!string.IsNullOrEmpty(searchName))
        {
            var lowerSearchName = searchName.ToLower();
            query = query.Where(u => EF.Functions.Like(u.Name.ToLower(), $"%{lowerSearchName}%"));
        }

        var totalUsers = await query.CountAsync();

        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResultDto<User>
        {
            Items = users,
            TotalCount = totalUsers
        };
    }

    public async Task<PaginatedResultDto<User>> GetVerificationRequestsAsync(string? searchName, int page, int pageSize)
    {
        var query = _context.Users
            .Where(u => u.VerificationRequested);

        if (!string.IsNullOrEmpty(searchName))
        {
            var lowerSearchName = searchName.ToLower();
            query = query.Where(u => EF.Functions.Like(u.Name.ToLower(), $"%{lowerSearchName}%"));
        }

        var totalItems = await query.CountAsync();
        var users = await query
            .OrderBy(u => u.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResultDto<User>
        {
            Items = users,
            TotalCount = totalItems
        };
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

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserProfileAsync(Guid userId, UpdateProfileDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        if (!string.IsNullOrEmpty(dto.Name))
            user.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.PhoneNumber))
            user.PhoneNumber = dto.PhoneNumber;

        if (!string.IsNullOrEmpty(dto.TelegramUsername))
            user.TelegramUsername = dto.TelegramUsername;

        await _context.SaveChangesAsync();
    }


    public async Task DeleteUserAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task BlockUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsBlocked = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task UnblockUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            user.IsBlocked = false;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RequestVerificationAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.VerificationRequested = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task VerifyUserAsync(Guid userId, bool isVerified)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.VerificationRequested = false;
            if (isVerified)
            {
                user.Role = "VerifiedUser";
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task ChangeUserRoleAsync(Guid userId, string newRole)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.Role = newRole;
            await _context.SaveChangesAsync();
        }
    }
}
