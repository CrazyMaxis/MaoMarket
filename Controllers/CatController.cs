using api.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatController : ControllerBase
    {
        private readonly CatService _catService;

        public CatController(CatService catService)
        {
            _catService = catService;
        }

        /// <summary>
        /// Получает кота по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор кота.</param>
        /// <response code="200">Возвращает кота.</response>
        /// <response code="404">Кот не найден.</response>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Получение кота по идентификатору", Description = "Получает кота по его идентификатору.")]
        [SwaggerResponse(200, "Возвращает кота.")]
        [SwaggerResponse(404, "Кот не найден.")]
        public async Task<IActionResult> GetCatById(Guid id)
        {
            var cat = await _catService.GetCatByIdAsync(id);
            if (cat == null) return NotFound("Кота с данным id не существует.");
            return Ok(cat);
        }

        /// <summary>
        /// Получает список котов из питомника с фильтрацией, поиском и пагинацией.
        /// </summary>
        /// <param name="filter">Фильтры для поиска и сортировки.</param>
        /// <response code="200">Возвращает список котов питомника.</response>
        [HttpGet("cattery")]
        [SwaggerOperation(Summary = "Получение котов питомника", Description = "Получает котов питомника с фильтрацией, поиском и пагинацией.")]
        [SwaggerResponse(200, "Возвращает список котов питомника.")]
        public async Task<IActionResult> GetCatteryCats([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] Guid? breedId = null, [FromQuery] string? searchName = null, [FromQuery] string? gender = null)
        {
            var result = await _catService.GetCatteryCatsAsync(page, pageSize, breedId, searchName, gender);
            return Ok(result);
        }


        /// <summary>
        /// Получает список котов по идентификатору пользователя.
        /// </summary>
        /// <param name="userId">Идентификатор пользователя.</param>
        /// <response code="200">Возвращает список котов.</response>
        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Получение котов пользователя по идентификатору", Description = "Получает список котов пользователя по его идентификатору.")]
        [SwaggerResponse(200, "Возвращает список котов.")]
        public async Task<IActionResult> GetCatsByUserId(Guid userId)
        {
            var cats = await _catService.GetCatsByUserIdAsync(userId);
            return Ok(cats);
        }

        /// <summary>
        /// Добавляет нового кота.
        /// </summary>
        /// <param name="createCatDto">Данные для создания нового кота.</param>
        /// <response code="201">Кот успешно добавлен.</response>
        [HttpPost]
        [Authorize]
        [SwaggerOperation(Summary = "Добавление нового кота", Description = "Добавляет нового кота.")]
        [SwaggerResponse(201, "Кот успешно добавлен.")]
        public async Task<IActionResult> AddCat([FromForm, SwaggerRequestBody(Description = "Данные кота")] CreateCatDto createCatDto)
        {
            var userId = Guid.Parse(User.Claims.First(c => c.Type == "id").Value);
            await _catService.AddCatAsync(createCatDto, userId);
            return Ok("Кот успешно добавлен.");
        }

        /// <summary>
        /// Обновляет информацию о коте.
        /// </summary>
        /// <param name="id">Идентификатор кота для обновления.</param>
        /// <param name="updateCatDto">Данные для обновления кота.</param>
        /// <response code="200">Информация о коте успешно обновлена.</response>
        /// <response code="404">Кот не найден.</response>
        [HttpPut("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Обновление информации о коте", Description = "Обновляет данные существующего кота.")]
        [SwaggerResponse(200, "Информация о коте успешно обновлена.")]
        [SwaggerResponse(404, "Кот не найден.")]
        public async Task<IActionResult> EditCat(Guid id, [FromForm, SwaggerRequestBody(Description = "Обновленные данные кота")] UpdateCatDto updateCatDto)
        {
            var result = await _catService.EditCatAsync(id, updateCatDto);
            if (result == null) return NotFound("Кота с данным id не существует.");
            return Ok("Информация о коте успешно обновлена.");
        }

        /// <summary>
        /// Удаляет кота по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор кота для удаления.</param>
        /// <response code="200">Кот успешно удален.</response>
        /// <response code="404">Кот не найден.</response>
        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Удаление кота", Description = "Удаляет кота по идентификатору.")]
        [SwaggerResponse(200, "Кот успешно удален.")]
        [SwaggerResponse(404, "Кот не найден.")]
        public async Task<IActionResult> DeleteCat(Guid id)
        {
            var result = await _catService.DeleteCatAsync(id);
            if (!result) return NotFound("Кота с данным id не существует.");
            return Ok("Кот успешно удален.");
        }

        /// <summary>
        /// Получает родословную кота.
        /// </summary>
        /// <param name="id">Идентификатор кота.</param>
        /// <response code="200">Возвращает родословную кота.</response>
        /// <response code="404">Кот не найден.</response>
        [HttpGet("{id}/pedigree")]
        [SwaggerOperation(Summary = "Получение родословной кота", Description = "Получает родословную кота по его идентификатору.")]
        [SwaggerResponse(200, "Возвращает родословную кота.")]
        [SwaggerResponse(404, "Кот не найден.")]
        public async Task<IActionResult> GetCatPedigree(Guid id)
        {
            var pedigree = await _catService.GetCatPedigreeAsync(id);
            if (pedigree == null) return NotFound("Кота с данным id не существует.");
            return Ok(pedigree);
        }
    }
}
