using api.Dto;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly CommentService _commentService;

    public CommentController(CommentService commentService)
    {
        _commentService = commentService;
    }

    /// <summary>
    /// Получение списка комментариев для поста.
    /// </summary>
    /// <param name="postId">Идентификатор поста.</param>
    /// <response code="200">Возвращает список комментариев для указанного поста.</response>
    [HttpGet("{postId}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Получение комментариев", Description = "Возвращает список комментариев для указанного поста.")]
    [SwaggerResponse(200, "Список комментариев.")]
    public async Task<IActionResult> GetComments(Guid postId)
    {
        var comments = await _commentService.GetCommentsByPostIdAsync(postId);
        
        return Ok(comments);
    }

    /// <summary>
    /// Создание нового комментария.
    /// </summary>
    /// <param name="commentDto">Данные нового комментария.</param>
    /// <response code="200">Комментарий успешно создан.</response>
    [HttpPost]
    [SwaggerOperation(Summary = "Создание комментария", Description = "Создает новый комментарий для указанного поста.")]
    [SwaggerResponse(200, "Комментарий успешно создан.")]
    public async Task<IActionResult> CreateComment([FromBody, SwaggerRequestBody(Description = "Текст нового комментария")] CommentDto commentDto)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "id").Value);

        var comment = new Comment
        {
            UserId = userId,
            PostId = commentDto.PostId,
            Body = commentDto.Body
        };

        await _commentService.CreateCommentAsync(comment);

        return Ok("Комментарий успешно создан.");
    }

    /// <summary>
    /// Обновление комментария.
    /// </summary>
    /// <param name="id">Идентификатор комментария.</param>
    /// <param name="newBody">Новый текст комментария.</param>
    /// <response code="200">Комментарий успешно обновлен.</response>
    /// <response code="403">Доступ запрещен.</response>
    /// <response code="404">Комментарий не найден.</response>
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Обновление комментария", Description = "Обновляет текст существующего комментария.")]
    [SwaggerResponse(200, "Комментарий успешно обновлен.")]
    [SwaggerResponse(403, "Доступ запрещен.")]
    [SwaggerResponse(404, "Комментарий не найден.")]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody, SwaggerRequestBody(Description = "Текст обновленного комментария")] string newBody)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "id").Value);
        var comment = await _commentService.GetCommentByIdAsync(id);

        if (comment == null)
            return NotFound("Комментария с данным id не существует.");

        if (comment.UserId != userId && !User.IsInRole("Administrator"))
            return Forbid("У вас нет прав удалить данный комментарий.");

        await _commentService.UpdateCommentAsync(id, newBody);

        return Ok("Комментарий успешно обновлен.");
    }

    /// <summary>
    /// Удаление комментария.
    /// </summary>
    /// <param name="id">Идентификатор комментария.</param>
    /// <response code="200">Комментарий успешно удален.</response>
    /// <response code="403">Доступ запрещен.</response>
    /// <response code="404">Комментарий не найден.</response>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Удаление комментария", Description = "Удаляет существующий комментарий.")]
    [SwaggerResponse(200, "Комментарий успешно удален.")]
    [SwaggerResponse(403, "Доступ запрещен.")]
    [SwaggerResponse(404, "Комментарий не найден.")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "id").Value);
        var comment = await _commentService.GetCommentByIdAsync(id);

        if (comment == null)
            return NotFound("Комментария с данным id не существует.");

        if (comment.UserId != userId && !User.IsInRole("Administrator"))
            return Forbid("У вас нет прав удалить данный комментарий.");

        await _commentService.DeleteCommentAsync(id);

        return Ok("Комментарий успешно удален.");
    }

    /// <summary>
    /// Добавляет реакцию на комментарий.
    /// </summary>
    /// <param name="id">Идентификатор комментария для реакции.</param>
    /// <param name="dto">Данные для реакции (Like или Dislike).</param>
    /// <response code="200">Реакция успешно добавлена.</response>
    /// <response code="404">Комментарий не найден.</response>
    [HttpPost("{commentId}/react")]
    [SwaggerOperation(Summary = "Реакция на комментарий", Description = "Добавляет лайк или дизлайк к комментарию.")]
    [SwaggerResponse(200, "Реакция успешно добавлена.")]
    [SwaggerResponse(404, "Комментарий не найден.")]
    public async Task<IActionResult> ReactToComment(Guid commentId, [FromBody, SwaggerRequestBody(Description = "Тип реакции Like или Dislike")] LikeDislikeDto dto)
    {
        var updatedComment = await _commentService.AddLikeDislikeToCommentAsync(commentId, dto.Action);

        if (updatedComment == null) return NotFound("Комментария с данным id не существует.");

        return Ok("Реакция успешно добавлена.");
    }
}
