﻿namespace Genesis;

using Genesis.Http;

public static partial class ExceptionExtensions {

    /// <summary>
    /// Convert an exception to a <seealso cref="ProblemDetails"/> object
    /// <code>
    /// var e = new Exception("Message 1", new("Message 0"));
    /// e.ToProblemDetails(1).Detail; // result is "Message 0"
    /// e.ToProblemDetails().Detail; // result is "Message 1"
    /// </code>
    /// </summary>
    /// <param name="exception">The exception to convert</param>
    /// <param name="maxMessageDepth">Optional maximum inner exception depth to use as the <see cref="ProblemDetails.Detail"/> property</param>
    /// <returns>A <see cref="ProblemDetails"/> with a <see cref="ProblemDetails.Detail"/> being <see cref="Exception.InnerException"/> message at the specified depth</returns>
    public static ProblemDetails ToProblemDetails(this Exception exception, ushort maxMessageDepth = 1) {
        for (ushort i = 0;
            i < maxMessageDepth && exception.InnerException is not null;
            i++, exception = exception.InnerException);

        return exception switch {
            ArgumentException e =>
                new ValidationProblemDetails(
                    new Dictionary<string, string[]> {
                        [e?.ParamName ?? string.Empty] = new[] { e?.Message ?? "Parameter could not be validated" }
                    }
                ) {
                    Title = "One or more validation errors have occurred.",
                    Type = HttpStatusCodes.Default[StatusCodes.Status400BadRequest],
                    Status = StatusCodes.Status400BadRequest,
                    Detail = e?.Message
                },
            ValidationException e => new ValidationProblemDetails(
                e.Errors.GroupBy(o => o.PropertyName)
                    .ToDictionary(k => k.Key, v => v.Select(o => o.ErrorMessage).ToArray())
            ) {
                Title = "One or more validation errors have occurred",
                Type = HttpStatusCodes.Default[StatusCodes.Status400BadRequest],
                Status = StatusCodes.Status400BadRequest,
                Detail = e.Message
            },
            System.ComponentModel.DataAnnotations.ValidationException e =>
                new ValidationProblemDetails(
                    e.ValidationResult.MemberNames.ToDictionary(
                        k => k, _ => new[] { e.ValidationResult.ErrorMessage ?? "Parameter could not be validated." }
                    )
                ) {
                    Title = "One or more validation errors have occurred.",
                    Type = HttpStatusCodes.Default[StatusCodes.Status400BadRequest],
                    Status = StatusCodes.Status400BadRequest,
                    Detail = e.Message
                },
            System.Diagnostics.UnreachableException e =>
                new ProblemDetails() {
                    Title = "An error occured that should not have been possible",
                    Detail = e.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = HttpStatusCodes.Default[StatusCodes.Status500InternalServerError]
                },
            Exception e =>
                new ProblemDetails() {
                    Title = "An unexpected error has occurred.",
                    Type = HttpStatusCodes.Default[StatusCodes.Status500InternalServerError],
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = e.Message
                }
        };
    }
}