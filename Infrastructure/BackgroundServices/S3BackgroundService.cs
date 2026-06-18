using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DIMS_Backend.Infrastructure.BackgroundServices;

public class S3BackgroundService : BackgroundService
{
    private readonly S3BackgroundQueue _queue;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3BackgroundService> _logger;
    private readonly string _bucketName;

    public S3BackgroundService(
        S3BackgroundQueue queue,
        IAmazonS3 s3Client,
        IConfiguration configuration,
        ILogger<S3BackgroundService> logger)
    {
        _queue = queue;
        _s3Client = s3Client;
        _logger = logger;
        _bucketName = configuration["AWS_BUCKET_NAME"] ?? configuration["AUDIT_BUCKET_NAME"] ?? "dims-assets-bucket";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("S3BackgroundService iniciado. Escuchando cola de subidas asíncronas...");

        await foreach (var job in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                var s3Key = string.IsNullOrEmpty(job.Folder) ? job.FileName : $"{job.Folder}/{job.FileName}";

                _logger.LogInformation("Subiendo archivo asíncronamente a S3: {BucketName}/{Key}", _bucketName, s3Key);

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = s3Key,
                    ContentBody = job.ContentBody,
                    ContentType = job.ContentType
                };

                await _s3Client.PutObjectAsync(putRequest, stoppingToken);
                _logger.LogInformation("Archivo subido con éxito a S3: {Key}", s3Key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir archivo a S3 asíncronamente");
            }
        }
    }
}
