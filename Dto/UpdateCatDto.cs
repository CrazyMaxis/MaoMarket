using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для обновления информации о коте.
/// </summary>
public class UpdateCatDto
{
    /// <summary>
    /// Описание кота.
    /// </summary>
    [SwaggerSchema(Description = "Описание кота")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Список идентификаторов фотографий, которые необходимо удалить.
    /// </summary>
    [SwaggerSchema(Description = "Список идентификаторов фотографий, которые необходимо удалить")]
    public List<Guid> PhotosToDelete { get; set; } = new();

    /// <summary>
    /// Из питомника.
    /// </summary>
    [SwaggerSchema(Description = "Из питомника")]
    public bool IsCattery { get; set; }

    /// <summary>
    /// Новый список фотографий кота для загрузки.
    /// </summary>
    [SwaggerSchema(Description = "Новый список фотографий кота для загрузки")]
    public List<IFormFile> NewPhotos { get; set; } = new();
}