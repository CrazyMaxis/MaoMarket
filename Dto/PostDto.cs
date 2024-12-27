using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace api.Dto;

/// <summary>
/// DTO для работы с постами.
/// </summary>
public class PostDto
{
    /// <summary>
    /// Заголовок поста.
    /// </summary>
    [Required(ErrorMessage = "Заголовок поста обязателен.")]
    [StringLength(100, ErrorMessage = "Заголовок поста не должен превышать 100 символов.")]
    [SwaggerSchema(Description = "Заголовок поста")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Содержание поста.
    /// </summary>
    [Required(ErrorMessage = "Содержание поста обязательно.")]
    [SwaggerSchema(Description = "Содержание поста")]
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Изображение для поста.
    /// </summary>
    [SwaggerSchema(Description = "Изображение для поста")]
    public IFormFile? Image { get; set; }
}
