using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для добавления новой породы.
/// </summary>
public class BreedDto
{
    /// <summary>
    /// Название породы.
    /// </summary>
    [SwaggerSchema(Description = "Название породы")]
    public string Name { get; set; } = string.Empty;
}
