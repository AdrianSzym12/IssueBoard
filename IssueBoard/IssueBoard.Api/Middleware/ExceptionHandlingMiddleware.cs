using System.Net;
using FluentValidation;
using IssueBoard.Api.Common.Errors;
using IssueBoard.Domain.Common;

namespace IssueBoard.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ValidationException exception)
        {
            await HandleValidationExceptionAsync(httpContext, exception);
        }
        catch (DomainException exception)
        {
            await HandleDomainExceptionAsync(httpContext, exception);
        }
        catch (InvalidOperationException exception)
        {
            await HandleInvalidOperationExceptionAsync(httpContext, exception);
        }
        catch (Exception exception)
        {
            await HandleUnexpectedExceptionAsync(httpContext, exception);
        }
    }

    private static async Task HandleValidationExceptionAsync(
        HttpContext httpContext,
        ValidationException exception)
    {
        IReadOnlyDictionary<string, string[]> errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        ApiErrorResponse response = new(
            Title: "Validation error",
            Status: StatusCodes.Status400BadRequest,
            Detail: "One or more validation errors occurred.",
            TraceId: httpContext.TraceIdentifier,
            Errors: errors);

        await WriteResponseAsync(httpContext, HttpStatusCode.BadRequest, response);
    }

    private static async Task HandleDomainExceptionAsync(
        HttpContext httpContext,
        DomainException exception)
    {
        ApiErrorResponse response = new(
            Title: "Domain error",
            Status: StatusCodes.Status400BadRequest,
            Detail: exception.Message,
            TraceId: httpContext.TraceIdentifier);

        await WriteResponseAsync(httpContext, HttpStatusCode.BadRequest, response);
    }

    private static async Task HandleInvalidOperationExceptionAsync(
        HttpContext httpContext,
        InvalidOperationException exception)
    {
        ApiErrorResponse response = new(
            Title: "Invalid operation",
            Status: StatusCodes.Status400BadRequest,
            Detail: exception.Message,
            TraceId: httpContext.TraceIdentifier);

        await WriteResponseAsync(httpContext, HttpStatusCode.BadRequest, response);
    }

    private async Task HandleUnexpectedExceptionAsync(
        HttpContext httpContext,
        Exception exception)
    {
        _logger.LogError(
            exception,
            "Unhandled exception occurred while processing request {Method} {Path}.",
            httpContext.Request.Method,
            httpContext.Request.Path);

        ApiErrorResponse response = new(
            Title: "Internal server error",
            Status: StatusCodes.Status500InternalServerError,
            Detail: "An unexpected error occurred.",
            TraceId: httpContext.TraceIdentifier);

        await WriteResponseAsync(httpContext, HttpStatusCode.InternalServerError, response);
    }

    private static async Task WriteResponseAsync(
        HttpContext httpContext,
        HttpStatusCode statusCode,
        ApiErrorResponse response)
    {
        if (httpContext.Response.HasStarted)
        {
            return;
        }

        httpContext.Response.Clear();
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = (int)statusCode;

        await httpContext.Response.WriteAsJsonAsync(response);
    }
}
