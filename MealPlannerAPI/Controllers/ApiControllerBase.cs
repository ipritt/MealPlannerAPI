using MealPlannerAPI.Models.Utility;
using Microsoft.AspNetCore.Mvc;

namespace MealPlannerAPI.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ActionResult<T> HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            // Handle multiple errors or pinpoint primary error type
            var primaryError = result.Errors[0] ?? Error.None;

            return primaryError.Type switch
            {
                ErrorType.Validation => HandleValidationErrors(result.Errors),
                ErrorType.NotFound => NotFound(CreateProblemDetails("Not Found", StatusCodes.Status404NotFound, primaryError.Description)),
                ErrorType.Conflict => Conflict(CreateProblemDetails("Conflict", StatusCodes.Status409Conflict, primaryError.Description)),
                _ => BadRequest(CreateProblemDetails("Bad Request", StatusCodes.Status400BadRequest, primaryError.Description))
            };
        }

        private BadRequestObjectResult HandleValidationErrors(IReadOnlyList<Error> errors)
        {
            // Extract validation format structured as field/code dictionaries
            var problemDetails = new ProblemDetails
            {
                Title = "Validation Failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Extensions =
                {
                    ["errors"] = errors.Select(e => new { e.Code, e.Description })
                }
            };

            return BadRequest(problemDetails);
        }

        private static ProblemDetails CreateProblemDetails(string title, int status, string detail) =>
            new()
            {
                Title = title,
                Status = status,
                Detail = detail
            };
    }
}
