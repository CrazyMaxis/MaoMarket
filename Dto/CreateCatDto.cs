using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace api.Dto;

/// <summary>
/// DTO для создания кота.
/// </summary>
public class CreateCatDto
{
    /// <summary>
    /// Имя кота.
    /// </summary>
    [Required(ErrorMessage = "Имя кота обязательно.")]
    [MaxLength(50, ErrorMessage = "Имя кота не может превышать 50 символов.")]
    [SwaggerSchema(Description = "Имя кота")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Пол кота (Male/Female).
    /// </summary>
    [Required(ErrorMessage = "Пол кота обязателен.")]
    [MaxLength(6, ErrorMessage = "Пол кота не может превышать 6 символов.")]
    [SwaggerSchema(Description = "Пол кота (Male/Female)")]
    public string Gender { get; set; } = string.Empty;

    /// <summary>
    /// Дата рождения кота.
    /// </summary>
    [Required(ErrorMessage = "Дата рождения обязательна.")]
    [SwaggerSchema(Description = "Дата рождения кота")]
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// ID породы кота.
    /// </summary>
    [Required(ErrorMessage = "Порода обязательна.")]
    [SwaggerSchema(Description = "ID породы кота")]
    public Guid BreedId { get; set; }

    /// <summary>
    /// Описание кота.
    /// </summary>
    [SwaggerSchema(Description = "Описание кота")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// ID отца (если указан).
    /// </summary>
    [SwaggerSchema(Description = "ID отца кота, если указан")]
    public Guid? FatherId { get; set; }

    /// <summary>
    /// ID матери (если указана).
    /// </summary>
    [SwaggerSchema(Description = "ID матери кота, если указана")]
    public Guid? MotherId { get; set; }

    /// <summary>
    /// Из питомника.
    /// </summary>
    [SwaggerSchema(Description = "Из питомника")]
    public bool IsCattery { get; set; } = false;

    /// <summary>
    /// Фотографии кота (массив файлов).
    /// </summary>
    [SwaggerSchema(Description = "Фотографии кота в формате массива файлов")]
    public List<IFormFile>? Photos { get; set; }
}
