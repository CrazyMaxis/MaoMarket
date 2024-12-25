using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для краткого отображения информации о посте.
/// </summary>
public class PostSummaryDto
{
    /// <summary>
    /// Идентификатор поста.
    /// </summary>
    [SwaggerSchema(Description = "Уникальный идентификатор поста")]
    public Guid Id { get; set; }

    /// <summary>
    /// Заголовок поста.
    /// </summary>
    [SwaggerSchema(Description = "Заголовок поста")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Краткое содержание поста (не более 100 символов).
    /// </summary>
    [SwaggerSchema(Description = "Краткое содержание поста (не более 100 символов)")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// URL изображения для поста.
    /// </summary>
    [SwaggerSchema(Description = "URL изображения для поста")]
    public string? Image { get; set; }
}
