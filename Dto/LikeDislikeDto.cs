using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace api.Dto;

/// <summary>
/// DTO для добавления лайка или дизлайка.
/// </summary>
public class LikeDislikeDto
{
    /// <summary>
    /// Тип действия: Like или Dislike.
    /// </summary>
    [Required(ErrorMessage = "Тип действия обязателен.")]
    [RegularExpression("^(Like|Dislike)$", ErrorMessage = "Тип действия должен быть 'Like' или 'Dislike'.")]
    [SwaggerSchema(Description = "Тип действия: Like для лайка или Dislike для дизлайка.")]
    public string Action { get; set; } = string.Empty;
}
