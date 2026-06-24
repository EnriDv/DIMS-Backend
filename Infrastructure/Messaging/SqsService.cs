using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DIMS_Backend.Infrastructure.Messaging;

public class SqsService : ISqsService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<SqsService> _logger;
    private readonly string _queueUrl;

    public SqsService(IAmazonSQS sqsClient, IConfiguration configuration, ILogger<SqsService> logger)
    {
        _sqsClient = sqsClient;
        _logger = logger;
        _queueUrl = configuration["SQS_QUEUE_URL"]
            ?? throw new InvalidOperationException("SQS_QUEUE_URL no está configurado.");
    }

    public async Task SendMessageAsync(string messageBody, CancellationToken cancellationToken = default)
    {
        var request = new SendMessageRequest
        {
            QueueUrl = _queueUrl,
            MessageBody = messageBody,
        };

        var response = await _sqsClient.SendMessageAsync(request, cancellationToken);
        _logger.LogInformation("Mensaje enviado a SQS. MessageId: {MessageId}", response.MessageId);
    }
}
