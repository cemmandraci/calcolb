using Calcolb.Shared.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Calcolb.API.Middleware;

public sealed class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validasyon hatası: {Path}", context.Request.Path);
            await WriteProblemDetails(context, StatusCodes.Status422UnprocessableEntity,
                "Validasyon Hatası",
                "Girilen veriler geçerli değil.",
                ex.Errors.Select(e => e.ErrorMessage).ToArray());
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Domain hatası: {Path}", context.Request.Path);
            await WriteProblemDetails(context, StatusCodes.Status400BadRequest,
                "İş Kuralı İhlali",
                ex.Message);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Kayıt bulunamadı: {Path}", context.Request.Path);
            await WriteProblemDetails(context, StatusCodes.Status404NotFound,
                "Kayıt Bulunamadı",
                ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen hata: {Path}", context.Request.Path);
            await WriteProblemDetails(context, StatusCodes.Status500InternalServerError,
                "Sunucu Hatası",
                "Beklenmeyen bir hata oluştu.");
        }
    }

    private static async Task WriteProblemDetails(
        HttpContext context,
        int statusCode,
        string title,
        string detail,
        string[]? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        if (errors is { Length: > 0 })
            problem.Extensions["errors"] = errors;

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
