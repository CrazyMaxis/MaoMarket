using api.Data;
using api.Dto;
using api.Models;
using Microsoft.EntityFrameworkCore;

public class AdvertisementService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MinioService _minioService;

    public AdvertisementService(ApplicationDbContext dbContext, MinioService minioService)
    {
        _dbContext = dbContext;
        _minioService = minioService;
    }

    public async Task<Advertisement> AddAdvertisementAsync(CreateAdvertisementDto createAdDto, Guid userId)
    {
        var ad = new Advertisement
        {
            Id = Guid.NewGuid(),
            Price = createAdDto.Price,
            CatId = createAdDto.CatId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Advertisements.Add(ad);
        await _dbContext.SaveChangesAsync();

        return ad;
    }

    public async Task<Advertisement?> UpdateAdvertisementAsync(Guid id, UpdateAdvertisementDto updateAdDto, Guid userId)
    {
        var ad = await _dbContext.Advertisements.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (ad == null) return null;

        ad.Price = updateAdDto.Price;

        await _dbContext.SaveChangesAsync();

        return ad;
    }

    public async Task<bool> DeleteAdvertisementAsync(Guid id, Guid userId, bool isAdmin)
    {
        var ad = await _dbContext.Advertisements.FirstOrDefaultAsync(a => a.Id == id && (a.UserId == userId || isAdmin));

        if (ad == null) return false;

        _dbContext.Advertisements.Remove(ad);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<List<AdvertisementResponseDto>> GetAdvertisementsAsync(int page, int pageSize, string? sortOrder, Guid? breedId, string? searchQuery, string? gender)
    {
        var query = _dbContext.Advertisements
            .Include(ad => ad.Cat)
            .ThenInclude(cat => cat.Photos)
            .Include(ad => ad.Cat.Breed)
            .AsQueryable();

        if (breedId.HasValue)
        {
            query = query.Where(ad => ad.Cat.BreedId == breedId);
        }

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(ad => ad.Cat.Name.Contains(searchQuery));
        }

        if (!string.IsNullOrEmpty(gender)) 
        { 
            query = query.Where(ad => ad.Cat.Gender == gender); 
        }

        query = sortOrder switch
        {
            "date" => query.OrderBy(ad => ad.CreatedAt),
            "age" => query.OrderBy(ad => ad.Cat.BirthDate),
            _ => query.OrderBy(ad => ad.CreatedAt)
        };

        var advertisements = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ad => new AdvertisementResponseDto
            {
                Id = ad.Id,
                Name = ad.Cat.Name,
                Breed = ad.Cat.Breed.Name,
                Gender = ad.Cat.Gender,
                Price = ad.Price,
                BirthDate = ad.Cat.BirthDate,
                PhotoUrl = ad.Cat.Photos.Any() ? _minioService.GetFileUrl(ad.Cat.Photos.First().Image) : string.Empty,
                CreatedAt = ad.CreatedAt
            })
            .ToListAsync();

        return advertisements;
    }

    public async Task<AdvertisementDetailsDto?> GetAdvertisementByIdAsync(Guid id)
    {
        var advertisement = await _dbContext.Advertisements
            .Include(ad => ad.Cat)
            .ThenInclude(cat => cat.Photos)
            .Include(ad => ad.Cat.Breed)
            .Include(ad => ad.User)
            .FirstOrDefaultAsync(ad => ad.Id == id);

        if (advertisement == null) return null;

        var advertisementDetailDto = new AdvertisementDetailsDto
        {
            Id = advertisement.Id,
            Price = advertisement.Price,
            CreatedAt = advertisement.CreatedAt,
            Cat = new CatDetailsDto
            {
                Id = advertisement.Cat.Id,
                Name = advertisement.Cat.Name,
                Gender = advertisement.Cat.Gender,
                Breed = advertisement.Cat.Breed.Name,
                Description = advertisement.Cat.Description,
                BirthDate = advertisement.Cat.BirthDate,
                Photos = advertisement.Cat.Photos.Select(p => new CatPhotoDto
                {
                    Id = p.Id,
                    Url = _minioService.GetFileUrl(p.Image)
                }).ToList()
            },
            User = new UserNameDto
            {
                Id = advertisement.User.Id,
                Name = advertisement.User.Name
            }
        };

        return advertisementDetailDto;
    }
}
