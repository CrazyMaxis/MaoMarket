using Microsoft.AspNetCore.Mvc;

[Route("api/minio")]
[ApiController]
public class MinioController : ControllerBase
{
    private readonly MinioService _minioService;

    public MinioController(MinioService minioService)
    {
        _minioService = minioService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Файл не загружен.");

        await using var stream = file.OpenReadStream();
        var objectName = await _minioService.UploadFileAsync(file.FileName, stream);
        var fileUrl = _minioService.GetFileUrl(objectName);

        return Ok(new { fileUrl });
    }
}
