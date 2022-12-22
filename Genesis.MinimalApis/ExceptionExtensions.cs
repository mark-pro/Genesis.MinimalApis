namespace Genesis;

public static partial class ExceptionExtensions {
    public static ProblemDetails ToProblemDetails(this Exception exception, ushort maxMessageDepth = 1) {

        for (ushort i = 0;
            i < maxMessageDepth && exception.InnerException is not null;
            i++, exception = exception.InnerException
        );

        return exception switch {
            ArgumentException aex =>
                new ValidationProblemDetails(
                    new Dictionary<string, string[]> {
                        [aex?.ParamName ?? string.Empty] = new[] { aex?.Message ?? "Parameter could not be validated" }
                    }
                ) {
                    Title = "One or more validation errors have occurred.",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = aex?.Message
                },
            System.ComponentModel.DataAnnotations.ValidationException ve =>
                new ValidationProblemDetails(
                    ve.ValidationResult.MemberNames.ToDictionary(
                        k => k, _ => new[] { ve.ValidationResult.ErrorMessage ?? "Parameter could not be validated." }
                    )
                ) {
                    Title = "One or more validation errors have occurred.",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = ve?.Message
                },
            Exception e =>
                new ProblemDetails() {
                    Title = "An unexpected error has occurred.",
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                    Detail = e.Message,
                    Status = StatusCodes.Status500InternalServerError
                }
        };
    }
}