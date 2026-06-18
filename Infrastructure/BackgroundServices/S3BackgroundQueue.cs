using System.Threading.Channels;

namespace DIMS_Backend.Infrastructure.BackgroundServices;

public class S3BackgroundQueue
{
    private readonly Channel<S3UploadJob> _channel;

    public S3BackgroundQueue()
    {
        _channel = Channel.CreateUnbounded<S3UploadJob>(new UnboundedChannelOptions
        {
            SingleReader = true
        });
    }

    public ChannelWriter<S3UploadJob> Writer => _channel.Writer;
    public ChannelReader<S3UploadJob> Reader => _channel.Reader;
}

public record S3UploadJob(string Folder, string FileName, string ContentBody, string ContentType);
