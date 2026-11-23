using GoatJersey_API.Helpers;

namespace GoatJersey_API.Middleware;

/// <summary>Convierte las excepciones de los Services en respuestas HTTP con { "error": "mensaje" }.</summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            int status;
            string message;

            switch (ex)
            {
                case BusinessRuleException:
                    status = StatusCodes.Status400BadRequest;
                    message = ex.Message;
                    break;
                case AppUnauthorizedException:
                    status = StatusCodes.Status401Unauthorized;
                    message = ex.Message;
                    break;
                case ForbiddenException:
                    status = StatusCodes.Status403Forbidden;
                    message = ex.Message;
                    break;
                case NotFoundException:
                    status = StatusCodes.Status404NotFound;
                    message = ex.Message;
                    break;
                default:
                    status = StatusCodes.Status500InternalServerError;
                    message = "Ocurrió un error inesperado en el servidor.";
                    _logger.LogError(ex, "Error no controlado");
                    break;
            }

            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = message });
        }
    }
}
