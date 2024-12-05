using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto
{
    /// <summary>
    /// DTO для входа пользователя в систему.
    /// </summary>
    public class CredentialsDto
    {
        /// <summary>
        /// Email пользователя.
        /// </summary>
        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный формат email.")]
        [SwaggerSchema(Description = "Email пользователя для входа.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        [Required(ErrorMessage = "Пароль обязателен.")]
        [SwaggerSchema(Description = "Пароль пользователя для входа.")]
        public string Password { get; set; } = string.Empty;
    }
}