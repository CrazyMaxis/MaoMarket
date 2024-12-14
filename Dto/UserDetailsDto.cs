using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto;

/// <summary>
/// DTO для получения деталей пользователя.
/// </summary>
public class UserDetailsDto
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    [SwaggerSchema(Description = "Идентификатор пользователя")]
    public Guid Id { get; set; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    [SwaggerSchema(Description = "Имя пользователя")]
    public string Name { get; set; } = string.Empty;
}
