using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace api.Dto;

/// <summary>
/// DTO для создания комментария.
/// </summary>
public class CommentDto
{
    /// <summary>
    /// Идентификатор поста, к которому добавляется комментарий.
    /// </summary>
    [Required(ErrorMessage = "Идентификатор поста обязателен.")]
    [SwaggerSchema(Description = "Идентификатор поста, к которому привязан комментарий.")]
    public Guid PostId { get; set; } = Guid.Empty;

    /// <summary>
    /// Текст комментария.
    /// </summary>
    [Required(ErrorMessage = "Текст комментария обязателен.")]
    [MaxLength(500, ErrorMessage = "Текст комментария не должен превышать 500 символов.")]
    [SwaggerSchema(Description = "Текст комментария.")]
    public string Body { get; set; } = string.Empty;
}
