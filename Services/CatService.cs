using api.Data;
using api.Dto;
using api.Models;
using Microsoft.EntityFrameworkCore;

public class CatService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly MinioService _minioService;

    public CatService(ApplicationDbContext dbContext, MinioService minioService)
    {
        _dbContext = dbContext;
        _minioService = minioService;
    }

    public async Task<CatDetailsDto> GetCatByIdAsync(Guid id)
    {
        var cat = await _dbContext.Cats
            .Include(c => c.Photos)
            .Include(c => c.Breed)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cat == null) throw new KeyNotFoundException("Cat not found.");

        return new CatDetailsDto
        {
            Id = cat.Id,
            Name = cat.Name,
            Gender = cat.Gender,
            Breed = cat.Breed.Name,
            BirthDate = cat.BirthDate,
            Description = cat.Description,
            Photos = cat.Photos.Select(p => new CatPhotoDto
            {
                Id = p.Id,
                Url = _minioService.GetFileUrl(p.Image)
            }).ToList()
        };
    }

    public async Task<List<ShortCatDto>> GetCatsWithoutAdvertisementsAsync(Guid userId)
    {
        var userRole = await _dbContext.Users
            .Where(user => user.Id == userId)
            .Select(user => user.Role)
            .FirstOrDefaultAsync();

        if (userRole == null)
        {
            throw new UnauthorizedAccessException("Пользователь не найден или не авторизован.");
        }

        var query = _dbContext.Cats
            .Include(cat => cat.Photos)
            .Include(cat => cat.Breed)
            .AsQueryable();

        if (userRole is "Administrator" or "Moderator")
        {
            query = query.Where(cat => cat.IsCattery || cat.UserId == userId);
        }
        else
        {
            query = query.Where(cat => cat.UserId == userId);
        }

        query = query.Where(cat => !_dbContext.Advertisements.Any(ad => ad.CatId == cat.Id));

        var cats = await query
            .Select(cat => new ShortCatDto
            {
                Id = cat.Id,
                Name = cat.Name,
                Gender = cat.Gender,
                Breed = cat.Breed,
                PhotoUrl = cat.Photos.Any()
                    ? _minioService.GetFileUrl(cat.Photos.First().Image)
                    : string.Empty
            })
            .ToListAsync();

        return cats;
    }


    public async Task<List<ShortCatDto>> GetCatsByUserIdAsync(Guid userId)
    {
        var cats = await _dbContext.Cats
            .Include(c => c.Photos)
            .Include(c => c.Breed)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return cats.Select(cat => new ShortCatDto
        {
            Id = cat.Id,
            Name = cat.Name,
            Gender = cat.Gender,
            Breed = new Breed
                {
                    Id = cat.Breed.Id,
                    Name = cat.Breed.Name
                },
            PhotoUrl = cat.Photos.FirstOrDefault()?.Image != null
                ? _minioService.GetFileUrl(cat.Photos.First().Image)
                : string.Empty
        }).ToList();
    }

    public async Task<PaginatedResultDto<ShortCatDto>> GetCatteryCatsAsync(int page, int pageSize, Guid? breedId, string? searchName, string? gender)
    {
        var query = _dbContext.Cats.Where(c => c.IsCattery);

        if (!string.IsNullOrEmpty(searchName))
        {
            var lowerSearchName = searchName.ToLower();
            query = query.Where(c => EF.Functions.Like(c.Name.ToLower(), $"%{lowerSearchName}%"));
        }

        if (!string.IsNullOrEmpty(gender))
        {
            var lowerGender = gender.ToLower();
            query = query.Where(c => c.Gender.ToLower() == lowerGender);
        }

        if (breedId.HasValue)
        {
            query = query.Where(c => c.BreedId == breedId.Value);
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(cat => new ShortCatDto
            {
                Id = cat.Id,
                Name = cat.Name,
                Gender = cat.Gender,
                Breed = new Breed
                {
                    Id = cat.Breed.Id,
                    Name = cat.Breed.Name
                },
                PhotoUrl = cat.Photos.Any()
                    ? _minioService.GetFileUrl(cat.Photos.First().Image)
                    : string.Empty
            })
            .ToListAsync();

        return new PaginatedResultDto<ShortCatDto>
        {
            Items = items,
            TotalCount = totalItems,
        };
    }

    public async Task AddCatAsync(CreateCatDto createCatDto, Guid userId)
    {
        
        var newCat = new Cat
        {
            Id = Guid.NewGuid(),
            Name = createCatDto.Name,
            Gender = createCatDto.Gender,
            BirthDate = createCatDto.BirthDate.ToUniversalTime(),
            BreedId = createCatDto.BreedId,
            Description = createCatDto.Description,
            IsCattery = createCatDto.IsCattery,
            FatherId = createCatDto.FatherId,
            MotherId = createCatDto.MotherId,
            UserId = userId
        };

        if (createCatDto.Photos != null && createCatDto.Photos.Any())
        {
            foreach (var photo in createCatDto.Photos)
            {
                using var stream = photo.OpenReadStream();
                var objectName = await _minioService.UploadFileAsync(photo.FileName, stream);

                newCat.Photos.Add(new CatPhoto
                {
                    CatId = newCat.Id,
                    Image = objectName
                });
            }
        }

        await _dbContext.Cats.AddAsync(newCat);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Cat?> EditCatAsync(Guid id, UpdateCatDto updateCatDto)
    {
        var cat = await _dbContext.Cats
            .Include(c => c.Photos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cat == null) return null;

        cat.Description = updateCatDto.Description;
        cat.IsCattery = updateCatDto.IsCattery;

        foreach (var photoId in updateCatDto.PhotosToDelete)
        {
            var photo = cat.Photos.FirstOrDefault(p => p.Id == photoId);
            if (photo != null)
            {
                await _minioService.DeleteFileAsync(photo.Image);
                _dbContext.CatPhotos.Remove(photo);
            }
        }

        foreach (var formFile in updateCatDto.NewPhotos)
        {
            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);
                stream.Position = 0;

                var photoUrl = await _minioService.UploadFileAsync(formFile.FileName, stream);
                var photo = new CatPhoto
                {
                    CatId = cat.Id,
                    Image = photoUrl
                };
                _dbContext.CatPhotos.Add(photo);
            }
        }

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            foreach (var entry in ex.Entries)
            {
                if (entry.Entity is Cat catEntry)
                {
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        _dbContext.Entry(catEntry).State = EntityState.Detached;
                        continue;
                    }

                    var currentValues = entry.CurrentValues;
                    var databaseValues = databaseEntry;
                    
                    foreach (var property in entry.Metadata.GetProperties()) 
                    { 
                        var currentValue = currentValues[property]; 
                        var databaseValue = databaseValues.GetValue<object>(property.Name); 
                        currentValues[property] = databaseValue; 
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        return cat;
    }


    public async Task<bool> DeleteCatAsync(Guid id)
    {
        var cat = await _dbContext.Cats
            .Include(c => c.Photos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cat == null) return false;

        foreach (var photo in cat.Photos)
        {
            await _minioService.DeleteFileAsync(photo.Image);
        }

        _dbContext.Cats.Remove(cat);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<PedigreeDto> GetCatPedigreeAsync(Guid id)
    {
        var cat = await _dbContext.Cats
            .Include(c => c.Photos)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cat == null) throw new KeyNotFoundException("Cat not found.");

        var mother = await GetParentDto(cat.MotherId);
        var father = await GetParentDto(cat.FatherId);

        var children = await _dbContext.Cats
            .Where(c => c.MotherId == id || c.FatherId == id)
            .Include(c => c.Photos)
            .ToListAsync();

        var childrenDtos = children.Select(c => new CatPedigreeDto
        {
            Id = c.Id,
            Name = c.Name,
            Gender = c.Gender,
            PhotoUrl = c.Photos.Any()
                ? _minioService.GetFileUrl(c.Photos.First().Image)
                : string.Empty
        }).ToList();


        return new PedigreeDto
        {
            Mother = mother,
            Father = father,
            Children = childrenDtos,
        };
    }

    private async Task<CatPedigreeDto?> GetParentDto(Guid? parentId)
    {
        if (parentId == null) return null;

        var parent = await _dbContext.Cats
            .Include(c => c.Photos)
            .FirstOrDefaultAsync(c => c.Id == parentId);

        if (parent == null) return null;

        return new CatPedigreeDto
        {
            Id = parent.Id,
            Name = parent.Name,
            Gender = parent.Gender,
            PhotoUrl = parent.Photos.FirstOrDefault()?.Image != null
                ? _minioService.GetFileUrl(parent.Photos.First().Image)
                : string.Empty
        };
    }
}
