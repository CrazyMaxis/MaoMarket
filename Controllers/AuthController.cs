using api.Dto;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtTokenService _tokenService;

    public AuthController(UserService userService, JwtTokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Регистрирует нового пользователя.
    /// </summary>
    /// <remarks>
    /// Пользователь будет создан с ролью "User" по умолчанию.
    /// </remarks>
    /// <param name="model">Данные для регистрации (имя, email, пароль).</param>
    /// <response code="200">Регистрация прошла успешно.</response>
    /// <response code="400">Пользователь с таким email уже существует.</response>
    [HttpPost("register")]
    [SwaggerOperation(Summary = "Регистрация нового пользователя", Description = "Создает нового пользователя с указанными данными.")]
    [SwaggerResponse(200, "Регистрация прошла успешно.")]
    [SwaggerResponse(400, "Пользователь с таким email уже существует.")]
    public async Task<IActionResult> Register([FromForm, SwaggerRequestBody(Description = "Данные нового пользователя")] RegisterDto model)
    {
        var existingUser = await _userService.GetByEmailAsync(model.Email);
        if (existingUser != null)
            return BadRequest("Пользователь с таким email уже существует.");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

        var newUser = new User
        {
            Name = model.Name,
            Email = model.Email,
            Password = hashedPassword,
            Role = "User"
        };

        await _userService.CreateUserAsync(newUser);

        return Ok("Регистрация прошла успешно.");
    }

    /// <summary>
    /// Аутентификация пользователя.
    /// </summary>
    /// <remarks>
    /// Если email и пароль совпадают, создается пара access и refresh токенов, которые передаются в cookies.
    /// </remarks>
    /// <param name="model">Email и пароль пользователя.</param>
    /// <response code="200">Успешный вход.</response>
    /// <response code="401">Неверный email или пароль.</response>
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Аутентификация пользователя", Description = "Проверяет учетные данные и возвращает токены.")]
    [SwaggerResponse(200, "Успешный вход.")]
    [SwaggerResponse(401, "Неверный email или пароль.")]
    public async Task<IActionResult> Login([FromBody, SwaggerRequestBody(Description = "Данные пользователя")] CredentialsDto model)
    {
        var user = await _userService.GetByEmailAsync(model.Email);
        if (user == null || !_userService.VerifyPassword(user, model.Password))
            return Unauthorized("Неверный email или пароль.");

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = new RefreshToken
        {
            Token = _tokenService.GenerateRefreshToken(),
            ExpiryTime = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        await _userService.AddRefreshTokenAsync(refreshToken);

        Response.Cookies.Append("AccessToken", accessToken, new CookieOptions { HttpOnly = true, Secure = true });
        Response.Cookies.Append("RefreshToken", refreshToken.Token, new CookieOptions { HttpOnly = true, Secure = true });

        return Ok(new { AccessToken = accessToken });
    }

    /// <summary>
    /// Обновляет access токен.
    /// </summary>
    /// <remarks>
    /// Использует refresh токен из cookies для генерации новой пары токенов.
    /// </remarks>
    /// <response code="200">Токены успешно обновлены.</response>
    /// <response code="401">Неверный или истекший refresh токен.</response>
    [HttpPost("refresh")]
    [SwaggerOperation(Summary = "Обновление токенов", Description = "Обновляет пару токенов на основе refresh токена.")]
    [SwaggerResponse(200, "Токены успешно обновлены.")]
    [SwaggerResponse(401, "Неверный или истекший refresh токен.")]
    public async Task<IActionResult> Refresh()
    {
        var refreshTokenValue = Request.Cookies["RefreshToken"];
        if (refreshTokenValue == null) return Unauthorized("Не предоставлен refresh-токен.");

        var refreshToken = await _userService.GetRefreshTokenAsync(refreshTokenValue);
        if (refreshToken == null || refreshToken.ExpiryTime <= DateTime.UtcNow)
            return Unauthorized("Неверный или истекший refresh-токен.");

        var user = refreshToken.User;
        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = new RefreshToken
        {
            Token = _tokenService.GenerateRefreshToken(),
            ExpiryTime = DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        await _userService.RemoveRefreshTokenAsync(refreshToken);
        await _userService.AddRefreshTokenAsync(newRefreshToken);

        Response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions { HttpOnly = true, Secure = true });
        Response.Cookies.Append("RefreshToken", newRefreshToken.Token, new CookieOptions { HttpOnly = true, Secure = true });

        return Ok(new { AccessToken = newAccessToken });
    }

    /// <summary>
    /// Выход из системы.
    /// </summary>
    /// <remarks>
    /// Удаляет refresh токен из базы данных и удаляет токены из cookies.
    /// </remarks>
    /// <response code="200">Успешный выход из системы.</response>
    [HttpPost("logout")]
    [SwaggerOperation(Summary = "Выход из системы", Description = "Удаляет токены из cookies и базы данных.")]
    [SwaggerResponse(200, "Успешный выход из системы.")]
    public async Task<IActionResult> Logout()
    {
        var refreshTokenValue = Request.Cookies["RefreshToken"];
        if (refreshTokenValue != null)
        {
            var refreshToken = await _userService.GetRefreshTokenAsync(refreshTokenValue);
            if (refreshToken != null)
            {
                await _userService.RemoveRefreshTokenAsync(refreshToken);
            }
        }

        Response.Cookies.Delete("AccessToken");
        Response.Cookies.Delete("RefreshToken");

        return Ok("Успешный выход из системы.");
    }
}
