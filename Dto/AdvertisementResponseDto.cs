using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для ответа о рекламе.
/// </summary>
public class AdvertisementResponseDto
{
    /// <summary>
    /// Идентификатор рекламы.
    /// </summary>
    [SwaggerSchema(Description = "Идентификатор рекламы")]
    public Guid Id { get; set; }

    /// <summary>
    /// Название рекламы.
    /// </summary>
    [SwaggerSchema(Description = "Название рекламы")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Порода животного, размещенного в рекламе.
    /// </summary>
    [SwaggerSchema(Description = "Порода животного")]
    public string Breed { get; set; } = string.Empty;

    /// <summary>
    /// Из питомника.
    /// </summary>
    [SwaggerSchema(Description = "Из питомника")]
    public bool IsCattery { get; set; } = false;

    /// <summary>
    /// Пол животного, размещенного в рекламе.
    /// </summary>
    [SwaggerSchema(Description = "Пол животного (Male/Female)")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// Цена рекламы.
    /// </summary>
    [SwaggerSchema(Description = "Цена рекламы")]
    public decimal Price { get; set; }

    /// <summary>
    /// Дата рождения животного, размещенного в рекламе.
    /// </summary>
    [SwaggerSchema(Description = "Дата рождения животного")]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// URL фотографии животного, размещенного в рекламе.
    /// </summary>
    [SwaggerSchema(Description = "URL фотографии животного")]
    public string PhotoUrl { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания рекламы.
    /// </summary>
    [SwaggerSchema(Description = "Дата создания рекламы")]
    public DateTime CreatedAt { get; set; }
}
