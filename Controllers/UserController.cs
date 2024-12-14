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
    /// Блокировка пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, которого необходимо заблокировать.</param>
    /// <response code="200">Пользователь успешно заблокирован.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    /// <response code="404">Пользователь с указанным ID не найден.</response>
    [HttpPost("{id}/block")]
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Moderator")]
    [SwaggerOperation(Summary = "Верификация пользователя", Description = "Модератор может верифицировать или отклонить пользователя, изменив его роль на VerifiedUser или User.")]
    public async Task<IActionResult> VerifyUser(Guid id, [FromQuery] bool isVerified)
    {
        await _userService.VerifyUserAsync(id, isVerified);
        return Ok(isVerified ? "Пользователь успешно верифицирован." : "Запрос на верификацию отклонен.");
    }

    /// <summary>
    /// Изменение роли пользователя администратором.
    /// </summary>
    /// <param name="id">Идентификатор пользователя, для которого нужно изменить роль.</param>
    /// <param name="newRole">Новая роль пользователя (например, Admin, User, Moderator, и т. д.).</param>
    /// <response code="200">Роль пользователя успешно изменена.</response>
    /// <response code="403">Нет прав для выполнения этой операции.</response>
    /// <response code="404">Пользователь с указанным ID не найден.</response>
    [HttpPost("{id}/change-role")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Изменение роли пользователя", Description = "Администратор может изменить роль пользователя.")]
    public async Task<IActionResult> ChangeUserRole(Guid id, [FromQuery] string newRole)
    {
        await _userService.ChangeUserRoleAsync(id, newRole);
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
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Удаление пользователя", Description = "Администратор может удалить пользователя.")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound("Пользователь не найден.");
        
        await _userService.DeleteUserAsync(user);
        return Ok("Пользователь успешно удален.");
    }
}
