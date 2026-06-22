using System.Security.Claims;
using IssueBoard.Api.Common.Errors;
using IssueBoard.Application.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace IssueBoard.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult<TValue>(Result<TValue> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return HandleFailure(result);
    }

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleFailure(result);
    }

    protected bool TryGetCurrentUserId(out Guid userId)
    {
        string? userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userIdValue, out userId);
    }

    protected IActionResult MissingUserIdClaim()
    {
        ApiErrorResponse response = new(
            Title: "Unauthorized",
            Status: StatusCodes.Status401Unauthorized,
            Detail: "Authenticated user id claim is missing.",
            TraceId: HttpContext.TraceIdentifier);

        return Unauthorized(response);
    }

    protected IActionResult HandleFailure(Result result)
    {
        ApiErrorResponse response = new(
            Title: GetTitle(result.Error.Type),
            Status: GetStatusCode(result.Error.Type),
            Detail: result.Error.Message,
            TraceId: HttpContext.TraceIdentifier);

        return StatusCode(response.Status, response);
    }

    private static int GetStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Failure => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetTitle(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => "Validation error",
            ErrorType.NotFound => "Resource not found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            ErrorType.Failure => "Request failed",
            _ => "Server error"
        };
    }
}
