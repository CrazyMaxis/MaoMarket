using api.Data;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class BreedService
{
    private readonly ApplicationDbContext _context;

    public BreedService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Breed>> GetBreedsAsync()
    {
        return await _context.Breeds.ToListAsync();
    }

    public async Task<Breed> AddBreedAsync(string name)
    {
        var breed = new Breed
        {
            Id = Guid.NewGuid(),
            Name = name
        };

        _context.Breeds.Add(breed);
        await _context.SaveChangesAsync();
        return breed;
    }

    public async Task<Breed?> UpdateBreedAsync(Guid id, string name)
    {
        var breed = await _context.Breeds.FirstOrDefaultAsync(b => b.Id == id);
        if (breed == null) return null;

        breed.Name = name;
        await _context.SaveChangesAsync();
        return breed;
    }

    public async Task<bool> DeleteBreedAsync(Guid id)
    {
        var breed = await _context.Breeds.FirstOrDefaultAsync(b => b.Id == id);
        if (breed == null) return false;

        _context.Breeds.Remove(breed);
        await _context.SaveChangesAsync();
        return true;
    }
}
