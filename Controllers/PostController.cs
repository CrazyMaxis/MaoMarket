using api.Dto;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Получает список постов с пагинацией, сортировкой, фильтрацией по хэштегам и поиском по названию.
    /// </summary>
    /// <param name="page">Номер страницы (по умолчанию 1).</param>
    /// <param name="pageSize">Размер страницы (по умолчанию 10).</param>
    /// <param name="sortDirection">Направление сортировки: asc или desc.</param>
    /// <param name="searchTitle">Поисковая строка по названию поста.</param>
    /// <response code="200">Возвращает список постов.</response>
    [HttpGet]
    [SwaggerOperation(Summary = "Получение списка постов", Description = "Получает список постов с пагинацией, сортировкой и фильтрацией.")]
    [SwaggerResponse(200, "Возвращает список постов.")]
    public async Task<IActionResult> GetPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortDirection = null, [FromQuery] string? searchTitle = null)
    {
        var posts = await _postService.GetPostsAsync(page, pageSize, sortDirection, searchTitle);
        
        return Ok(posts);
    }

    /// <summary>
    /// Получает пост по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор поста.</param>
    /// <response code="200">Возвращает пост.</response>
    /// <response code="404">Пост не найден.</response>
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Получение поста по идентификатору", Description = "Получает пост по его идентификатору.")]
    [SwaggerResponse(200, "Возвращает пост.")]
    [SwaggerResponse(404, "Пост не найден.")]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        var post = await _postService.GetPostByIdAsync(id);
        if (post == null) return NotFound("Поста с данным id не существует.");
        return Ok(post);
    }

    /// <summary>
    /// Создает новый пост.
    /// </summary>
    /// <param name="postDto">Данные поста для создания.</param>
    /// <response code="200">Пост успешно создан.</response>
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [SwaggerOperation(Summary = "Создание нового поста", Description = "Создает новый пост с данными, предоставленными в запросе.")]
    [SwaggerResponse(200, "Пост успешно создан.")]
    public async Task<IActionResult> CreatePost([FromForm, SwaggerRequestBody(Description = "Данные нового поста")] PostDto postDto)
    {
        await _postService.CreatePostAsync(postDto);
        
        return Ok("Пост успешно создан.");
    }

    /// <summary>
    /// Обновляет существующий пост.
    /// </summary>
    /// <param name="id">Идентификатор поста для обновления.</param>
    /// <param name="postDto">Данные для обновления поста.</param>
    /// <response code="200">Пост успешно обновлен.</response>
    /// <response code="404">Пост не найден.</response>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    [SwaggerOperation(Summary = "Обновление поста", Description = "Обновляет существующий пост.")]
    [SwaggerResponse(200, "Пост успешно обновлен.")]
    [SwaggerResponse(404, "Пост не найден.")]
    public async Task<IActionResult> UpdatePost(Guid id, [FromForm, SwaggerRequestBody(Description = "Обновленные данные поста")] PostDto postDto)
    {
        var updatedPost = await _postService.UpdatePostAsync(id, postDto);

        if (updatedPost == null) return NotFound("Поста с данным id не существует.");

        return Ok("Пост успешно обновлен.");
    }

    /// <summary>
    /// Удаляет пост.
    /// </summary>
    /// <param name="id">Идентификатор поста для удаления.</param>
    /// <response code="200">Пост успешно удален.</response>
    /// <response code="404">Пост не найден.</response>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    [SwaggerOperation(Summary = "Удаление поста", Description = "Удаляет пост по идентификатору.")]
    [SwaggerResponse(200, "Пост успешно удален.")]
    [SwaggerResponse(404, "Пост не найден.")]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var result = await _postService.DeletePostAsync(id);
        if (!result) return NotFound("Поста с данным id не существует.");

        return Ok("Пост успешно удален.");
    }
}
