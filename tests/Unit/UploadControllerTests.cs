using Amazon.S3;
using Amazon.S3.Model;
using DIMS_Backend.Controllers;
using DIMS_Backend.Tests.Behavior;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DIMS_Backend.Tests.Unit;

public class UploadControllerTests
{
    private readonly IConfiguration _configuration;

    public UploadControllerTests()
    {
        var settings = new Dictionary<string, string?>
        {
            { "AWS_BUCKET_NAME", "dims-test-bucket" }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Fact]
    public async Task UploadImage_WithValidImage_ReturnsOkWithUrl()
    {
        // Arrange
        var fakeS3 = FakeS3Proxy.Create(shouldFail: false);
        var controller = new UploadController(fakeS3, _configuration, NullLogger<UploadController>.Instance);

        var fileBytes = Encoding.UTF8.GetBytes("fake image content");
        var file = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "file", "test-image.png")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/png"
        };

        // Act
        var result = await controller.UploadImage(file);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        
        var value = okResult.Value;
        var urlProp = value.GetType().GetProperty("url");
        Assert.NotNull(urlProp);
        var url = urlProp.GetValue(value) as string;
        Assert.StartsWith("https://dims-test-bucket.s3.amazonaws.com/originals/", url);
        Assert.EndsWith(".png", url);
    }

    [Fact]
    public async Task UploadImage_WithNullFile_ReturnsBadRequest()
    {
        // Arrange
        var fakeS3 = FakeS3Proxy.Create();
        var controller = new UploadController(fakeS3, _configuration, NullLogger<UploadController>.Instance);

        // Act
        var result = await controller.UploadImage(null!);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var value = badRequestResult.Value;
        var messageProp = value.GetType().GetProperty("message");
        Assert.NotNull(messageProp);
        var message = messageProp.GetValue(value) as string;
        Assert.Equal("No se proporcionó ningún archivo o el archivo está vacío.", message);
    }

    [Fact]
    public async Task UploadImage_WithInvalidExtension_ReturnsBadRequest()
    {
        // Arrange
        var fakeS3 = FakeS3Proxy.Create();
        var controller = new UploadController(fakeS3, _configuration, NullLogger<UploadController>.Instance);

        var fileBytes = Encoding.UTF8.GetBytes("fake file content");
        var file = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "file", "test.txt")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };

        // Act
        var result = await controller.UploadImage(file);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var value = badRequestResult.Value;
        var messageProp = value.GetType().GetProperty("message");
        Assert.NotNull(messageProp);
        var message = messageProp.GetValue(value) as string;
        Assert.Equal("Formato de archivo no válido. Solo se admiten JPG, PNG y WebP.", message);
    }

    [Fact]
    public async Task UploadImage_WhenS3Throws_ReturnsInternalServerError()
    {
        // Arrange
        var fakeS3 = FakeS3Proxy.Create(shouldFail: true);
        var controller = new UploadController(fakeS3, _configuration, NullLogger<UploadController>.Instance);

        var fileBytes = Encoding.UTF8.GetBytes("fake image content");
        var file = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "file", "test-image.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        // Act
        var result = await controller.UploadImage(file);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        var value = statusCodeResult.Value;
        var messageProp = value.GetType().GetProperty("message");
        Assert.NotNull(messageProp);
        var message = messageProp.GetValue(value) as string;
        Assert.Equal("Error interno al subir la imagen", message);
    }
}
