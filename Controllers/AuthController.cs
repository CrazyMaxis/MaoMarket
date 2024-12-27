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
    private readonly VerificationCodeService _codeService;
    private readonly EmailService _emailService;

    public AuthController(UserService userService, JwtTokenService tokenService, VerificationCodeService codeService, EmailService emailService)
    {
        _userService = userService;
        _tokenService = tokenService;
         _codeService = codeService;
        _emailService = emailService;
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

        var verificationCode = new VerificationCode
        {
            UserId = newUser.Id,
            Code = new Random().Next(100000, 999999).ToString(),
            ExpiryTime = DateTime.UtcNow.AddMinutes(10)
        };

        await _codeService.CreateVerificationCodeAsync(verificationCode);

        await _emailService.SendEmailAsync(newUser.Email, "Ваш код подтверждения", $"Ваш код подтверждения: {verificationCode.Code}");

        var userResponse = new User
        {
            Id = newUser.Id,
            Name = newUser.Name,
            Email = newUser.Email,
            PhoneNumber = newUser.PhoneNumber,
            TelegramUsername = newUser.TelegramUsername,
            Role = newUser.Role,
            IsBlocked = newUser.IsBlocked,
            VerificationRequested = newUser.VerificationRequested
        };

        return Ok(new { User = userResponse });
    }

    /// <summary>
    /// Подтверждение email пользователя с использованием кода верификации.
    /// </summary>
    /// <remarks>
    /// Пользователь должен ввести код, который был отправлен на его email во время регистрации.
    /// Если код правильный и не истек, email будет подтвержден, и пользователю будут выданы токены доступа и обновления.
    /// </remarks>
    /// <param name="model">Данные для верификации (userId и код).</param>
    /// <response code="200">Email успешно подтвержден, токены выданы.</response>
    /// <response code="400">Неверный или истекший код верификации.</response>
    /// <response code="404">Пользователь не найден.</response>
    [HttpPost("verify")]
    [SwaggerOperation(Summary = "Подтверждение email пользователя", Description = "Проверяет код верификации и подтверждает email пользователя.")]
    [SwaggerResponse(200, "Email успешно подтвержден, токены выданы.")]
    [SwaggerResponse(400, "Неверный или истекший код верификации.")]
    [SwaggerResponse(404, "Пользователь не найден.")]
    public async Task<IActionResult> Verify([FromBody] VerifyDto model)
    {
        var verificationCode = await _codeService.GetVerificationCodeAsync(model.UserId, model.Code);
        if (verificationCode == null || verificationCode.ExpiryTime < DateTime.UtcNow)
            return BadRequest("Неверный или истекший код.");

        await _codeService.DeleteAllVerificationCodesForUserAsync(model.UserId);

        var user = await _userService.GetByIdAsync(model.UserId);
        if (user == null) return NotFound("Пользователь не найден.");

        user.IsEmailVerified = true;
        await _userService.UpdateUserAsync(user);

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

        return Ok("Email верифицирован");
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
    /// <response code="423">Ваш аккаунт заблокирован.</response>
    [HttpPost("login")]
    [SwaggerOperation(Summary = "Аутентификация пользователя", Description = "Проверяет учетные данные и возвращает токены.")]
    [SwaggerResponse(200, "Успешный вход.")]
    [SwaggerResponse(401, "Неверный email или пароль.")]
    [SwaggerResponse(423, "Ваш аккаунт заблокирован.")]

    public async Task<IActionResult> Login([FromBody, SwaggerRequestBody(Description = "Данные пользователя")] CredentialsDto model)
    {
        var user = await _userService.GetByEmailAsync(model.Email);
        if (user == null || !_userService.VerifyPassword(user, model.Password))
            return Unauthorized("Неверный email или пароль.");

        if (user.IsBlocked)
            return StatusCode(423, "Ваш аккаунт заблокирован.");

        var userResponse = new User
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            TelegramUsername = user.TelegramUsername,
            Role = user.Role,
            IsBlocked = user.IsBlocked,
            VerificationRequested = user.VerificationRequested
        };

        if (!user.IsEmailVerified) {
            var verificationCode = new VerificationCode
            {
                UserId = user.Id,
                Code = new Random().Next(100000, 999999).ToString(),
                ExpiryTime = DateTime.UtcNow.AddMinutes(10)
            };

            await _codeService.CreateVerificationCodeAsync(verificationCode);

            await _emailService.SendEmailAsync(user.Email, "Ваш код подтверждения", $"Ваш код подтверждения: {verificationCode.Code}");

            return StatusCode(403, new { User = userResponse });
        }

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

        return Ok(new { User = userResponse });
    }

    /// <summary>
    /// Обновляет access токен.
    /// </summary>
    /// <remarks>
    /// Использует refresh токен из cookies для генерации новой пары токенов.
    /// </remarks>
    /// <response code="200">Токены успешно обновлены.</response>
    /// <response code="401">Неверный или истекший refresh токен.</response>
    /// <response code="423">Ваш аккаунт заблокирован.</response>
    [HttpPost("refresh")]
    [SwaggerOperation(Summary = "Обновление токенов", Description = "Обновляет пару токенов на основе refresh токена.")]
    [SwaggerResponse(200, "Токены успешно обновлены.")]
    [SwaggerResponse(401, "Неверный или истекший refresh токен.")]
    [SwaggerResponse(423, "Ваш аккаунт заблокирован.")]
    public async Task<IActionResult> Refresh()
    {
        var refreshTokenValue = Request.Cookies["RefreshToken"];
        if (refreshTokenValue == null) return Unauthorized("Не предоставлен refresh-токен.");

        var refreshToken = await _userService.GetRefreshTokenAsync(refreshTokenValue);
        if (refreshToken == null || refreshToken.ExpiryTime <= DateTime.UtcNow)
            return Unauthorized("Неверный или истекший refresh-токен.");

        var user = refreshToken.User;

        if (user.IsBlocked)
            return StatusCode(423, "Ваш аккаунт заблокирован.");

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

        var userResponse = new User
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            TelegramUsername = user.TelegramUsername,
            Role = user.Role,
            IsBlocked = user.IsBlocked,
            VerificationRequested = user.VerificationRequested
        };

        return Ok(new { User = userResponse });
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
