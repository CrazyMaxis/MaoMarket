using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для полного описания кота.
/// </summary>
public class CatDetailsDto
{
    /// <summary>
    /// Идентификатор кота.
    /// </summary>
    [SwaggerSchema(Description = "Идентификатор кота")]
    public Guid Id { get; set; }

    /// <summary>
    /// Имя кота.
    /// </summary>
    [SwaggerSchema(Description = "Имя кота")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Пол кота (Male/Female).
    /// </summary>
    [SwaggerSchema(Description = "Пол кота (Male/Female)")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// Порода кота.
    /// </summary>
    [SwaggerSchema(Description = "Порода кота")]
    public string Breed { get; set; } = string.Empty;

    /// <summary>
    /// Дата рождения кота.
    /// </summary>
    [SwaggerSchema(Description = "Дата рождения кота")]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Описание кота.
    /// </summary>
    [SwaggerSchema(Description = "Описание кота")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Список фотографий кота с ID и URL.
    /// </summary>
    [SwaggerSchema(Description = "Список фотографий кота с ID и URL")]
    public List<CatPhotoDto> Photos { get; set; } = new();
}

/// <summary>
/// DTO для фотографии кота.
/// </summary>
public class CatPhotoDto
{
    /// <summary>
    /// Идентификатор фотографии.
    /// </summary>
    [SwaggerSchema(Description = "Идентификатор фотографии кота")]
    public Guid Id { get; set; }

    /// <summary>
    /// URL фотографии.
    /// </summary>
    [SwaggerSchema(Description = "URL фотографии кота")]
    public string Url { get; set; } = string.Empty;
}
