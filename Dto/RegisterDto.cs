using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Dto
{
    /// <summary>
    /// DTO для регистрации нового пользователя.
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Имя пользователя.
        /// </summary>
        [Required(ErrorMessage = "Имя обязательно.")]
        [MaxLength(50, ErrorMessage = "Имя не может превышать 50 символов.")]
        [SwaggerSchema(Description = "Имя пользователя.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Email пользователя.
        /// </summary>
        [Required(ErrorMessage = "Email обязателен.")]
        [EmailAddress(ErrorMessage = "Некорректный формат email.")]
        [SwaggerSchema(Description = "Email пользователя.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        [Required(ErrorMessage = "Пароль обязателен.")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать не менее 6 символов.")]
        [SwaggerSchema(Description = "Пароль пользователя.")]
        public string Password { get; set; } = string.Empty;
    }
}