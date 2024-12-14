using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для создания рекламы.
/// </summary>
public class CreateAdvertisementDto
{
    /// <summary>
    /// Цена рекламы.
    /// </summary>
    [Required(ErrorMessage = "Цена рекламы обязательна.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше нуля.")]
    [SwaggerSchema(Description = "Цена рекламы")]
    public decimal Price { get; set; }

    /// <summary>
    /// Идентификатор кота, который размещается в рекламе.
    /// </summary>
    [Required(ErrorMessage = "Идентификатор кота обязателен.")]
    [SwaggerSchema(Description = "Идентификатор кота, который размещается в рекламе")]
    public Guid CatId { get; set; }
}
