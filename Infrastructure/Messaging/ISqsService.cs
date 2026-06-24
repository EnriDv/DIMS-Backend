namespace DIMS_Backend.Infrastructure.Messaging;

public interface ISqsService
{
    Task SendMessageAsync(string messageBody, CancellationToken cancellationToken = default);
}
