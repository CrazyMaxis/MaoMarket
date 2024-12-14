using api.Services;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers;

/// <summary>
/// Контроллер для управления породами.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class BreedsController : ControllerBase
{
    private readonly BreedService _breedService;

    public BreedsController(BreedService breedService)
    {
        _breedService = breedService;
    }

    /// <summary>
    /// Получить список всех пород.
    /// </summary>
    /// <response code="200">Возвращает список пород.</response>
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Получить список всех пород.")]
    [SwaggerResponse(200, "Возвращает список пород.")]
    public async Task<IActionResult> GetBreeds()
    {
        var breeds = await _breedService.GetBreedsAsync();

        return Ok(breeds);
    }

    /// <summary>
    /// Добавить новую породу.
    /// </summary>
    /// <param name="name">Название породы для создания.</param>
    /// <response code="200">Пост успешно создан.</response>
    [HttpPost]
    [SwaggerOperation(Summary = "Добавить новую породу.", Description = "Создает новую породу с данными, предоставленными в запросе.")]
    [SwaggerResponse(200, "Порода успешно добавлена.")]
    public async Task<IActionResult> AddBreed([FromForm, SwaggerRequestBody(Description = "Название новой породы")] string name)
    {
        await _breedService.AddBreedAsync(name);

        return Ok("Порода успешна добавлена.");
    }

    /// <summary>
    /// Обновить породу.
    /// </summary>
    /// <param name="id">Идентификатор породы для обновления.</param>
    /// <param name="name">Название для обновления породы.</param>
    /// <response code="200">Пост успешно обновлен.</response>
    /// <response code="404">Пост не найден.</response>
    [HttpPut("{id:guid}")]
    [SwaggerOperation(Summary = "Обновить породу.", Description = "Обновляет существующую породу.")]
    [SwaggerResponse(200, "Порода успешно обновлена.")]
    [SwaggerResponse(404, "Порода не найдена.")]
    public async Task<IActionResult> UpdateBreed(Guid id, [FromForm, SwaggerRequestBody(Description = "Название обновленной породы")] string name)
    {
        var breed = await _breedService.UpdateBreedAsync(id, name);
        if (breed == null) return NotFound("Порода с данным id не существует.");

        return Ok("Порода успешно обновлена.");
    }

    /// <summary>
    /// Удалить породу.
    /// </summary>
    /// <param name="id">Идентификатор поста для удаления.</param>
    /// <response code="200">Порода успешно удалена.</response>
    /// <response code="404">Порода не найдена.</response>
    [HttpDelete("{id:guid}")]
    [SwaggerOperation(Summary = "Удалить породу.", Description = "Удаляет породу по идентификатору.")]
    [SwaggerResponse(200, "Порода успешно удалена.")]
    [SwaggerResponse(404, "Порода не найдена.")]
    public async Task<IActionResult> DeleteBreed(Guid id)
    {
        var result = await _breedService.DeleteBreedAsync(id);
        if (!result) return NotFound("Порода с данным id не существует.");

        return Ok("Порода успешно удалена.");
    }
}
