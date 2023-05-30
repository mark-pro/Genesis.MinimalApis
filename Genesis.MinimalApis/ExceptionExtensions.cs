namespace Genesis;

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
    public static ProblemDetails ToProblemDetails(this Exception exception, Func<Exception, Option<ProblemDetails>>? customMap = null, ushort maxMessageDepth = 1) {
        var ex = Enumerable.Range(0, maxMessageDepth)
            .Fold(exception, (e, _) => e.InnerException ?? e);

        var f = (Exception e) => () => e switch {
            ValidationException ve =>
                new ValidationProblemDetails(ve.Errors.GroupBy(o => o.PropertyName).ToDictionary(k => k.Key, v => v.Select(o => o.ErrorMessage).ToArray())) {
                    Title = "One or more validation errors have occurred",
                    Type = HttpStatusCodes.Default[StatusCodes.Status400BadRequest],
                    Status = StatusCodes.Status400BadRequest,
                    Detail = exception.Message
                },
            ArgumentException e =>
                new ValidationProblemDetails(
                    new Dictionary<string, string[]> {
                        [e?.ParamName ?? string.Empty] = new[] { e?.Message ?? "Parameter could not be validated" }
                    }
                ) {
                    Title = "Invalid argument provided.",
                    Type = HttpStatusCodes.Default[StatusCodes.Status400BadRequest],
                    Status = StatusCodes.Status400BadRequest,
                    Detail = e?.Message
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
#if NET7_0_OR_GREATER
            System.Diagnostics.UnreachableException e =>
                new ProblemDetails() {
                    Title = "An error occurred that should not have been possible",
                    Detail = e.Message,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = HttpStatusCodes.Default[StatusCodes.Status500InternalServerError]
                },
#endif
            Exception e =>
                new ProblemDetails() {
                    Title = "An unexpected error has occurred.",
                    Type = HttpStatusCodes.Default[StatusCodes.Status500InternalServerError],
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = e.Message
                }
        };

        return Optional(customMap?.Invoke(ex)).Flatten().IfNone(f(ex));
    }
}