using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DIMS_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<UploadController> _logger;
    private readonly string _bucketName;

    public UploadController(IAmazonS3 s3Client, IConfiguration configuration, ILogger<UploadController> logger)
    {
        _s3Client = s3Client;
        _logger = logger;
        _bucketName = configuration["S3_BUCKET_NAME"]
            ?? configuration["AWS_BUCKET_NAME"]
            ?? configuration["AUDIT_BUCKET_NAME"]
            ?? "dims-assets-bucket";
    }

    [HttpPost]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No se proporcionó ningún archivo o el archivo está vacío." });
        }

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png" && fileExtension != ".webp")
        {
            return BadRequest(new { message = "Formato de archivo no válido. Solo se admiten JPG, PNG y WebP." });
        }

        var fileName = $"{Guid.NewGuid()}{fileExtension}";
        var s3Key = $"originals/{fileName}";

        _logger.LogInformation("Subiendo archivo {FileName} a S3 ({BucketName}/{Key})", file.FileName, _bucketName, s3Key);

        try
        {
            using var stream = file.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = s3Key,
                InputStream = stream,
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(putRequest);

            var publicUrl = $"https://{_bucketName}.s3.amazonaws.com/{s3Key}";
            _logger.LogInformation("Subida exitosa. URL pública: {Url}", publicUrl);

            return Ok(new { url = publicUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falla al subir imagen a S3");
            return StatusCode(500, new { message = "Error interno al subir la imagen", details = ex.Message });
        }
    }
}
