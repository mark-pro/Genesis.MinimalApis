namespace Genesis;

public static partial class ExceptionExtensions {
    public static ProblemDetails ToProblemDetails(this Exception exception) =>
        exception switch {
            ArgumentException aex =>
                new ValidationProblemDetails(
                    new Dictionary<string, string[]> {
                        [aex?.ParamName ?? string.Empty] = new[] { aex?.Message ?? "Parameter could not be validated" }
                    }
                ) {
                    Title = "One or more validation errors have occurred",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                    Status = StatusCodes.Status400BadRequest
                },
            System.ComponentModel.DataAnnotations.ValidationException ve =>
                new ValidationProblemDetails(
                    ve.ValidationResult.MemberNames.ToDictionary(
                        k => k, _ => new[] { ve.ValidationResult.ErrorMessage ?? "Parameter could not be validated." }
                    )
                ) {
                    Title = "One or more validation errors have occurred.",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                    Status = StatusCodes.Status400BadRequest
                },
            Exception e =>
                new() {
                    Title = "An unexpected error has occurred.",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                    Detail = e.Message,
                    Status = StatusCodes.Status500InternalServerError
                }
        };
}