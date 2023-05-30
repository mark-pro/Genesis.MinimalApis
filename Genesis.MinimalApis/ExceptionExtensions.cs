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
    public static ProblemDetails ToProblemDetails(this ValidationException exception) =>
        new ValidationProblemDetails(
            exception.Errors.GroupBy(o => o.PropertyName)
                    .ToDictionary(k => k.Key, v => v.Select(o => o.ErrorMessage).ToArray())
            ) {
                Title = "One or more validation errors have occurred",
                Type = HttpStatusCodes.Default[StatusCodes.Status400BadRequest],
                Status = StatusCodes.Status400BadRequest,
                Detail = exception.Message
            };
}