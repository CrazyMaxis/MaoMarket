using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для обновления информации о рекламе.
/// </summary>
public class UpdateAdvertisementDto
{
    /// <summary>
    /// Цена рекламы.
    /// </summary>
    [Required(ErrorMessage = "Цена рекламы обязательна.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше нуля.")]
    [SwaggerSchema(Description = "Цена рекламы")]
    public decimal Price { get; set; }
}
