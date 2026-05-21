using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // 1. Loguear el error real para ti (el desarrollador)
        _logger.LogError(exception, "Ocurrió un error no manejado: {Message}", exception.Message);

        // 2. Preparar el JSON limpio para el Frontend
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Error interno del servidor",
            Detail = "Ocurrió un problema al procesar tu solicitud. Por favor, intenta más tarde."
        };

        // Si es un error de validación tuyo, puedes cambiar el status a 400
        if (exception is ArgumentException)
        {
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Solicitud inválida";
            problemDetails.Detail = exception.Message;
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Le decimos a .NET que ya manejamos el error
    }
}