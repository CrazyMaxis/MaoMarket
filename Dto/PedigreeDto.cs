using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для описания предков и потомков кота.
/// </summary>
public class CatPedigreeDto
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

/// <summary>
/// DTO для отображения полного генеалогического древа кота.
/// </summary>
public class PedigreeDto
{
    /// <summary>
    /// Мать кота.
    /// </summary>
    [SwaggerSchema(Description = "Мать кота")]
    public CatPedigreeDto? Mother { get; set; }

    /// <summary>
    /// Отец кота.
    /// </summary>
    [SwaggerSchema(Description = "Отец кота")]
    public CatPedigreeDto? Father { get; set; }

    /// <summary>
    /// Список потомков кота.
    /// </summary>
    [SwaggerSchema(Description = "Список потомков кота")]
    public List<CatPedigreeDto> Children { get; set; } = new();
}