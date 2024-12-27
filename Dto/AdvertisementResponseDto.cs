using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для ответа о объявлении.
/// </summary>
public class AdvertisementResponseDto
{
    /// <summary>
    /// Идентификатор объявления.
    /// </summary>
    [SwaggerSchema(Description = "Идентификатор объявления")]
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор животного.
    /// </summary>
    [SwaggerSchema(Description = "Идентификатор объявления")]
    public Guid CatId { get; set; }

    /// <summary>
    /// Идентификатор создателя объявления.
    /// </summary>
    [SwaggerSchema(Description = "Идентификатор создателя")]
    public Guid UserId { get; set; }

    /// <summary>
    /// Название объявления.
    /// </summary>
    [SwaggerSchema(Description = "Название объявления")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Порода животного, размещенного в объявлении.
    /// </summary>
    [SwaggerSchema(Description = "Порода животного")]
    public string Breed { get; set; } = string.Empty;

    /// <summary>
    /// Из питомника.
    /// </summary>
    [SwaggerSchema(Description = "Из питомника")]
    public bool IsCattery { get; set; } = false;

    /// <summary>
    /// Пол животного, размещенного в объявлении.
    /// </summary>
    [SwaggerSchema(Description = "Пол животного (Male/Female)")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// Цена объявления.
    /// </summary>
    [SwaggerSchema(Description = "Цена объявления")]
    public decimal Price { get; set; }

    /// <summary>
    /// Дата рождения животного, размещенного в объявлении.
    /// </summary>
    [SwaggerSchema(Description = "Дата рождения животного")]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// URL фотографии животного, размещенного в объявлении.
    /// </summary>
    [SwaggerSchema(Description = "URL фотографии животного")]
    public string PhotoUrl { get; set; } = string.Empty;

    /// <summary>
    /// Дата создания объявления.
    /// </summary>
    [SwaggerSchema(Description = "Дата создания объявления")]
    public DateTime CreatedAt { get; set; }
}
