using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для получения деталей рекламы.
/// </summary>
public class AdvertisementDetailsDto
{
    /// <summary>
    /// Идентификатор рекламы.
    /// </summary>
    [SwaggerSchema(Description = "Идентификатор рекламы")]
    public Guid Id { get; set; }

    /// <summary>
    /// Цена рекламы.
    /// </summary>
    [SwaggerSchema(Description = "Цена рекламы")]
    public decimal Price { get; set; }

    /// <summary>
    /// Дата создания рекламы.
    /// </summary>
    [SwaggerSchema(Description = "Дата создания рекламы")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Детали кота, размещенного в рекламе.
    /// </summary>
    [SwaggerSchema(Description = "Детали кота, размещенного в рекламе")]
    public CatDetailsDto Cat { get; set; } = null!;

    /// <summary>
    /// Детали пользователя, разместившего рекламу.
    /// </summary>
    [SwaggerSchema(Description = "Детали пользователя, разместившего рекламу")]
    public UserNameDto User { get; set; } = null!;
}
