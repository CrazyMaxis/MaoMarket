using api.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Route("api/[controller]")]
[ApiController]
public class AdvertisementController : ControllerBase
{
    private readonly AdvertisementService _adService;

    public AdvertisementController(AdvertisementService adService)
    {
        _adService = adService;
    }

    /// <summary>
    /// Добавление нового объявления.
    /// </summary>
    /// <param name="createAdDto">Данные нового объявления.</param>
    /// <response code="201">Объявление успешно добавлено.</response>
    [HttpPost]
    [Authorize]
    [SwaggerOperation(Summary = "Добавление нового объявления", Description = "Добавляет новое объявление.")]
    [SwaggerResponse(201, "Объявление успешно добавлено.")]
    public async Task<IActionResult> AddAdvertisement([FromForm, SwaggerRequestBody(Description = "Данные нового объявления")] CreateAdvertisementDto createAdDto)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "id").Value);
        await _adService.AddAdvertisementAsync(createAdDto, userId);
        return Ok("Объявление успешно добавлено.");
    }

    /// <summary>
    /// Редактирование объявления.
    /// </summary>
    /// <param name="id">Идентификатор объявления.</param>
    /// <param name="updateAdDto">Данные для обновления объявления.</param>
    /// <response code="200">Объявление успешно обновлено.</response>
    /// <response code="404">Объявление не найдено.</response>
    [HttpPut("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Редактирование объявления", Description = "Редактирует существующее объявление.")]
    [SwaggerResponse(200, "Объявление успешно обновлено.")]
    [SwaggerResponse(404, "Объявление не найдено.")]
    public async Task<IActionResult> UpdateAdvertisement(Guid id, [FromForm, SwaggerRequestBody(Description = "Данные обновленного объявления")] UpdateAdvertisementDto updateAdDto)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "id").Value);
        var result = await _adService.UpdateAdvertisementAsync(id, updateAdDto, userId);

        if (result == null) return NotFound("Объявление не найдено или у вас нет прав на его редактирование.");
        return Ok("Объявление успешно обновлено.");
    }

    /// <summary>
    /// Удаление объявления.
    /// </summary>
    /// <param name="id">Идентификатор объявления.</param>
    /// <response code="200">Объявление успешно удалено.</response>
    /// <response code="404">Объявление не найдено.</response>
    [HttpDelete("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Удаление объявления", Description = "Удаляет объявление.")]
    [SwaggerResponse(200, "Объявление успешно удалено.")]
    [SwaggerResponse(404, "Объявление не найдено.")]
    public async Task<IActionResult> DeleteAdvertisement(Guid id)
    {
        var userId = Guid.Parse(User.Claims.First(c => c.Type == "id").Value);
        var isAdmin = User.Claims.Any(c => c.Type == "role" && c.Value == "admin");
        var result = await _adService.DeleteAdvertisementAsync(id, userId, isAdmin);

        if (!result) return NotFound("Объявление не найдено или у вас нет прав на его удаление.");
        return Ok("Объявление успешно удалено.");
    }

    /// <summary>
    /// Получение объявлений с фильтрацией и сортировкой.
    /// </summary>
    /// <param name="page">Номер страницы для пагинации.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <param name="sortOrder">Параметр сортировки (по умолчанию — "date").</param>
    /// <param name="breedId">Идентификатор породы для фильтрации.</param>
    /// <param name="searchQuery">Запрос для поиска.</param>
    /// <param name="gender">Пол кота для фильтрации.</param>
    /// <response code="200">Объявления успешно получены.</response>
    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Получение объявлений с фильтрацией и сортировкой", Description = "Возвращает список объявлений с фильтрацией и сортировкой.")]
    [SwaggerResponse(200, "Объявления успешно получены.")]
    public async Task<IActionResult> GetAdvertisements([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? sortOrder = "date", [FromQuery] Guid? breedId = null, [FromQuery] string? searchQuery = null, [FromQuery] string? gender = null)
    {
        var ads = await _adService.GetAdvertisementsAsync(page, pageSize, sortOrder, breedId, searchQuery, gender);
        return Ok(ads);
    }

    /// <summary>
    /// Получение объявления по ID.
    /// </summary>
    /// <param name="id">Идентификатор объявления.</param>
    /// <response code="200">Объявление успешно получено.</response>
    /// <response code="404">Объявление не найдено.</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Получение объявления по ID", Description = "Возвращает объявление по его ID.")]
    [SwaggerResponse(200, "Объявление успешно получено.")]
    [SwaggerResponse(404, "Объявление не найдено.")]
    public async Task<IActionResult> GetAdvertisementById(Guid id)
    {
        var ad = await _adService.GetAdvertisementByIdAsync(id);
        if (ad == null) return NotFound("Объявление не найдено.");
        return Ok(ad);
    }
}
