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

    public async Task<Advertisement?> UpdateAdvertisementAsync(Guid id, UpdateAdvertisementDto updateAdDto)
    {
        var ad = await _dbContext.Advertisements.FirstOrDefaultAsync(a => a.Id == id);

        if (ad == null) return null;

        ad.Price = updateAdDto.Price;

        await _dbContext.SaveChangesAsync();

        return ad;
    }

    public async Task<bool> DeleteAdvertisementAsync(Guid id)
    {
        var ad = await _dbContext.Advertisements.FirstOrDefaultAsync(a => a.Id == id);

        if (ad == null) return false;

        _dbContext.Advertisements.Remove(ad);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<PaginatedResultDto<AdvertisementResponseDto>> GetAdvertisementsAsync(
    int page,
    int pageSize,
    string sortBy,
    string sortOrder,
    Guid? breedId,
    string? searchName,
    string? gender,
    bool? isCattery)
    {
        var query = _dbContext.Advertisements
            .Include(ad => ad.Cat)
            .ThenInclude(cat => cat.Photos)
            .Include(ad => ad.Cat.Breed)
            .AsQueryable();

        if (breedId.HasValue)
        {
            query = query.Where(ad => ad.Cat.BreedId == breedId.Value);
        }

        if (!string.IsNullOrEmpty(searchName))
        {
            var lowerSearchName = searchName.ToLower();
            query = query.Where(ad => EF.Functions.Like(ad.Cat.Name.ToLower(), $"%{lowerSearchName}%"));
        }

        if (!string.IsNullOrEmpty(gender))
        {
            var lowerGender = gender.ToLower();
            query = query.Where(ad => ad.Cat.Gender.ToLower() == lowerGender);
        }

        if (isCattery.HasValue)
        {
            query = query.Where(ad => ad.Cat.IsCattery == isCattery.Value);
        }

        query = sortBy.ToLower() switch
        {
            "price" => sortOrder.ToLower() == "asc"
                ? query.OrderBy(ad => ad.Price)
                : query.OrderByDescending(ad => ad.Price),
            "birthdate" => sortOrder.ToLower() == "asc"
                ? query.OrderBy(ad => ad.Cat.BirthDate)
                : query.OrderByDescending(ad => ad.Cat.BirthDate),
            _ => sortOrder.ToLower() == "asc"
                ? query.OrderBy(ad => ad.CreatedAt)
                : query.OrderByDescending(ad => ad.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var advertisements = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ad => new AdvertisementResponseDto
            {
                Id = ad.Id,
                CatId = ad.Cat.Id,
                UserId = ad.UserId,
                Name = ad.Cat.Name,
                Breed = ad.Cat.Breed.Name,
                IsCattery = ad.Cat.IsCattery,
                Gender = ad.Cat.Gender,
                Price = ad.Price,
                BirthDate = ad.Cat.BirthDate,
                PhotoUrl = ad.Cat.Photos.Any()
                    ? _minioService.GetFileUrl(ad.Cat.Photos.First().Image)
                    : string.Empty,
                CreatedAt = ad.CreatedAt
            })
            .ToListAsync();

        return new PaginatedResultDto<AdvertisementResponseDto>
        {
            Items = advertisements,
            TotalCount = totalCount
        };
    }


    public async Task<AdvertisementSimpleDto?> GetAdvertisementByIdAsync(Guid id)
    {
        var advertisement = await _dbContext.Advertisements
            .Select(ad => new AdvertisementSimpleDto
            {
                Id = ad.Id,
                Price = ad.Price,
                CatId = ad.CatId
            })
            .FirstOrDefaultAsync(ad => ad.Id == id);

        return advertisement;
    }
}
