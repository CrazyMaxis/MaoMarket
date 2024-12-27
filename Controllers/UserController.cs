using api.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Получение пользователей с фильтрацией, пагинацией и поиском.
    /// </summary>
    /// <param name="pageNumber">Номер страницы (по умолчанию 1).</param>
    /// <param name="pageSize">Размер страницы (по умолчанию 10).</param>
    /// <param name="role">Фильтр по роли (например, Admin, User и т. д.).</param>
    /// <param name="isBlocked">Фильтр по статусу блокировки (true - заблокирован, false - активен).</param>
    /// <param name="searchName">Поиск по имени.</param>
    /// <response code="200">Список пользователей успешно получен.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    [HttpGet]
    [Authorize(Roles = "Administrator")]
    [SwaggerOperation(Summary = "Получение пользователей с фильтрацией и пагинацией", Description = "Администратор может получить список пользователей с фильтрацией по роли, статусу блокировки, поиском по имени и пагинацией.")]
    public async Task<IActionResult> GetUsersFiltered(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        [FromQuery] string? role = null, 
        [FromQuery] bool? isBlocked = null, 
        [FromQuery] string? searchName = null)
    {
        var result = await _userService.GetUsersAsync(page, pageSize, role, isBlocked, searchName);
        return Ok(result);
    }

    /// <summary>
    /// Получение пользователей с заявками на верификацию.
    /// </summary>
    /// <param name="pageNumber">Номер страницы (по умолчанию 1).</param>
    /// <param name="pageSize">Размер страницы (по умолчанию 10).</param>
    /// <param name="searchName">Поиск по имени.</param>
    /// <response code="200">Список пользователей успешно получен.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    [HttpGet("verification-requests")]
    [Authorize(Roles = "Administrator, Moderator")]
    [SwaggerOperation(Summary = "Получение пользователей с заявками на верификацию", Description = "Возвращает пользователей, которые отправили заявку на верификацию, с поддержкой пагинации и поиска по имени.")]
    public async Task<IActionResult> GetVerificationRequests(
        [FromQuery] string? searchName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _userService.GetVerificationRequestsAsync(searchName, page, pageSize);
        return Ok(result);
    }


    /// <summary>
    /// Обновление профиля пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, профиль которого нужно обновить.</param>
    /// <param name="dto">Новые данные пользователя.</param>
    /// <response code="200">Профиль пользователя успешно обновлено.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    /// <response code="404">Пользователь с указанным ID не найден.</response>
    [HttpPut("{id}/update-profile")]
    [Authorize]
    [SwaggerOperation(Summary = "Обновление профиля пользователя", Description = "Позволяет пользователю обновить имя, номер телефона и имя пользователя в Telegram.")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateProfileDto dto)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound("Пользователь не найден.");

        if (!User.Claims.Any(c => c.Type == "id" && c.Value == id.ToString()))
            return Forbid("Вы не можете изменить чужой профиль.");

        await _userService.UpdateUserProfileAsync(id, dto);

        return Ok("Профиль успешно обновлен.");
    }


    /// <summary>
    /// Блокировка пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, которого необходимо заблокировать.</param>
    /// <response code="200">Пользователь успешно заблокирован.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    /// <response code="404">Пользователь с указанным ID не найден.</response>
    [HttpPost("{id}/block")]
    [Authorize(Roles = "Administrator")]
    [SwaggerOperation(Summary = "Блокировка пользователя", Description = "Этот метод позволяет администратору заблокировать пользователя по его идентификатору.")]
    public async Task<IActionResult> BlockUser(Guid id)
    {
        await _userService.BlockUserAsync(id);
        return Ok("Пользователь успешно заблокирован.");
    }

    /// <summary>
    /// Разблокировка пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, которого необходимо разблокировать.</param>
    /// <response code="200">Пользователь успешно разблокирован.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    /// <response code="404">Пользователь с указанным ID не найден.</response>
    [HttpPost("{id}/unblock")]
    [Authorize(Roles = "Administrator")]
    [SwaggerOperation(Summary = "Разблокировка пользователя", Description = "Этот метод позволяет администратору разблокировать пользователя по его идентификатору.")]
    public async Task<IActionResult> UnblockUser(Guid id)
    {
        await _userService.UnblockUserAsync(id);
        return Ok("Пользователь успешно разблокирован.");
    }

    /// <summary>
    /// Запрос на верификацию пользователя.
    /// </summary>
    /// <remarks>
    /// Этот метод позволяет пользователю отправить запрос на верификацию. После отправки запроса роль пользователя изменяется на «PendingVerification» до завершения процесса.
    /// </remarks>
    /// <response code="200">Запрос на верификацию успешно отправлен.</response>
    /// <response code="401">Пользователь не авторизован.</response>
    [HttpPost("request-verification")]
    [Authorize]
    [SwaggerOperation(Summary = "Запрос на верификацию", Description = "Позволяет пользователю отправить запрос на верификацию.")]
    public async Task<IActionResult> RequestVerification()
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "id").Value);
        await _userService.RequestVerificationAsync(userId);
        return Ok("Запрос на верификацию успешно отправлен.");
    }

    /// <summary>
    /// Верификация пользователя модератором.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, которого нужно верифицировать или отклонить.</param>
    /// <param name="isVerified">Флаг, который указывает, верифицирован ли пользователь (true) или нет (false).</param>
    /// <response code="200">Пользователь успешно верифицирован/отклонен.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    /// <response code="404">Пользователь с указанным ID не найден.</response>
    [HttpPost("{id}/verify")]
    [Authorize(Roles = "Administrator, Moderator")]
    [SwaggerOperation(Summary = "Верификация пользователя", Description = "Модератор может верифицировать или отклонить пользователя, изменив его роль на VerifiedUser или User.")]
    public async Task<IActionResult> VerifyUser(Guid id, [FromBody] VerifyUserRequestDto request)
    {
        if (request == null)
            return BadRequest("Тело запроса не должно быть пустым.");

        await _userService.VerifyUserAsync(id, request.IsVerified);
        return Ok(request.IsVerified 
            ? "Пользователь успешно верифицирован." 
            : "Запрос на верификацию отклонен.");
    }


    /// <summary>
    /// Изменение роли пользователя администратором.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, для которого нужно изменить роль.</param>
    /// <param name="dto">DTO с новой ролью.</param>
    /// <response code="200">Роль пользователя успешно изменена.</response>
    /// <response code="400">Некорректные данные или недопустимая роль.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    /// <response code="404">Пользователь с указанным ID не найден.</response>
    [HttpPost("{id}/change-role")]
    [Authorize(Roles = "Administrator")]
    [SwaggerOperation(Summary = "Изменение роли пользователя", Description = "Администратор может изменить роль пользователя.")]
    public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeRoleDto dto)
    {
        if (string.IsNullOrEmpty(dto.NewRole))
        {
            return BadRequest("Роль не может быть пустой.");
        }

        var allowedRoles = new[] { "Administrator", "Moderator", "NewsEditor", "VerifiedUser", "User" };

        if (!allowedRoles.Contains(dto.NewRole))
        {
            return BadRequest($"Недопустимая роль. Разрешённые роли: {string.Join(", ", allowedRoles)}.");
        }

        await _userService.ChangeUserRoleAsync(id, dto.NewRole);
        return Ok("Роль пользователя успешно изменена.");
    }


    /// <summary>
    /// Удаление пользователя администратором.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, которого нужно удалить.</param>
    /// <response code="200">Пользователь успешно удален.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    /// <response code="404">Пользователь с указанным ID не найден.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    [SwaggerOperation(Summary = "Удаление пользователя", Description = "Администратор может удалить пользователя.")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound("Пользователь не найден.");
        
        await _userService.DeleteUserAsync(user);
        return Ok("Пользователь успешно удален.");
    }
}
