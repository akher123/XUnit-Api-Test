using Api;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ResultHttpExtensions
{
    public static IActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return ToErrorActionResult(controller, result.Error);
    }

    public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return ToErrorActionResult(controller, result.Error);
    }

    private static ObjectResult ToErrorActionResult(ControllerBase controller, Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest,
        };

        return new ObjectResult(new { code = error.Code, message = error.Message })
        {
            StatusCode = statusCode,
        };
    }
}
