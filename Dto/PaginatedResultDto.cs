using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для результата с пагинацией.
/// </summary>
/// <typeparam name="T">Тип элементов в результате.</typeparam>
public class PaginatedResultDto<T>
{
    /// <summary>
    /// Коллекция элементов текущей страницы.
    /// </summary>
    [SwaggerSchema(Description = "Коллекция элементов текущей страницы.")]
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Общее количество элементов.
    /// </summary>
    [SwaggerSchema(Description = "Общее количество элементов.")]
    public int TotalCount { get; set; }
}
