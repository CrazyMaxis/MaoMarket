using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

public class VerificationCodeService
{
    private readonly ApplicationDbContext _context;

    public VerificationCodeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateVerificationCodeAsync(VerificationCode code)
    {
        _context.VerificationCodes.Add(code);
        await _context.SaveChangesAsync();
    }

    public async Task<VerificationCode?> GetVerificationCodeAsync(Guid userId, string code)
    {
        return await _context.VerificationCodes
            .SingleOrDefaultAsync(vc => vc.UserId == userId && vc.Code == code);
    }

    public async Task DeleteVerificationCodeAsync(VerificationCode code)
    {
        _context.VerificationCodes.Remove(code);
        await _context.SaveChangesAsync();
    }
}
