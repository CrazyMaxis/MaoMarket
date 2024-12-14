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
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cat == null) throw new KeyNotFoundException("Cat not found.");

        return new CatDetailsDto
        {
            Id = cat.Id,
            Name = cat.Name,
            Gender = cat.Gender,
            BirthDate = cat.BirthDate,
            Description = cat.Description,
            Photos = cat.Photos.Select(p => new CatPhotoDto
            {
                Id = p.Id,
                Url = _minioService.GetFileUrl(p.Image)
            }).ToList()
        };
    }

    public async Task<List<UserCatDto>> GetCatsByUserIdAsync(Guid userId)
    {
        var cats = await _dbContext.Cats
            .Include(c => c.Photos)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return cats.Select(cat => new UserCatDto
        {
            Id = cat.Id,
            Name = cat.Name,
            Gender = cat.Gender,
            PhotoUrl = cat.Photos.FirstOrDefault()?.Image != null
                ? _minioService.GetFileUrl(cat.Photos.First().Image)
                : string.Empty
        }).ToList();
    }

    public async Task AddCatAsync(CreateCatDto createCatDto, Guid userId)
    {
        
        var newCat = new Cat
        {
            Id = Guid.NewGuid(),
            Name = createCatDto.Name,
            Gender = createCatDto.Gender,
            BirthDate = createCatDto.BirthDate,
            BreedId = createCatDto.BreedId,
            Description = createCatDto.Description,
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

        var partner = await GetPartner(cat);

        return new PedigreeDto
        {
            Mother = mother,
            Father = father,
            Children = childrenDtos,
            Partner = partner
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

   private async Task<CatPedigreeDto?> GetPartner(Cat cat)
    {
        var firstChild = await _dbContext.Cats
            .Where(c => c.MotherId == cat.Id || c.FatherId == cat.Id)
            .Include(c => c.Photos)
            .FirstOrDefaultAsync();

        if (firstChild == null) return null;

        Guid? partnerId = firstChild.MotherId == cat.Id ? firstChild.FatherId : firstChild.MotherId;
        if (partnerId == null) return null;

        var partner = await _dbContext.Cats
            .Include(c => c.Photos)
            .FirstOrDefaultAsync(c => c.Id == partnerId);

        if (partner == null) return null;

        return new CatPedigreeDto
        {
            Id = partner.Id,
            Name = partner.Name,
            Gender = partner.Gender,
            PhotoUrl = partner.Photos.FirstOrDefault()?.Image != null
                ? _minioService.GetFileUrl(partner.Photos.First().Image)
                : string.Empty
        };
    }
}
