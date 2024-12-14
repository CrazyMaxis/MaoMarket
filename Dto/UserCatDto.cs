using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для кота, который принадлежит пользователю.
/// </summary>
public class UserCatDto
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
    /// Ссылка на фотографию кота.
    /// </summary>
    [SwaggerSchema(Description = "Ссылка на фотографию кота")]
    public string PhotoUrl { get; set; } = string.Empty;
}