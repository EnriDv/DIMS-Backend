using DIMS_Backend.Infrastructure.BackgroundServices;
using Xunit;
using System.Threading.Tasks;

namespace DIMS_Backend.Tests.Unit;

public class S3BackgroundQueueTests
{
    [Fact]
    public async Task Queue_ShouldWriteAndReadJobsSuccessfully()
    {
        // Arrange
        var queue = new S3BackgroundQueue();
        var job = new S3UploadJob("test-folder", "test.json", "{}", "application/json");

        // Act
        var writeResult = queue.Writer.TryWrite(job);
        var readJob = await queue.Reader.ReadAsync();

        // Assert
        Assert.True(writeResult);
        Assert.Equal("test-folder", readJob.Folder);
        Assert.Equal("test.json", readJob.FileName);
        Assert.Equal("{}", readJob.ContentBody);
        Assert.Equal("application/json", readJob.ContentType);
    }
}
