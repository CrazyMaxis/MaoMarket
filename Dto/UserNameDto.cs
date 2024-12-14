using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для представления информации о пользователе (ID и имя).
/// </summary>
public class UserNameDto
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    [SwaggerSchema(Description = "Уникальный идентификатор пользователя.")]
    public Guid Id { get; set; } = Guid.Empty;

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [SwaggerSchema(Description = "Имя пользователя.")]
    public string Name { get; set; } = string.Empty;
}
