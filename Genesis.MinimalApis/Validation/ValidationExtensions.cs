using Microsoft.AspNetCore.Builder;

namespace Genesis.Validation;

public static class RouteHandlerBuilderExtensions {

    /// <summary>
    /// Add a validation filter to a route based on the base type of the <seealso cref="ValidationFilter" />.
    /// </summary>
    /// <param name="builder">The route to apply the filter to</param>
    /// <typeparam name="T">The generic type of the filter to use</typeparam>
    /// <returns>An instance of a <seealso cref="RouteHandlerBuilder" />with the filter applied.</returns>
    public static RouteHandlerBuilder AddValidationFilter<T>(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter<ValidationFilter<T>>();

    /// <summary>
    /// Converts a <seealso cref="ValidationResult" /> to a <seealso cref="ValidationProblemDetails" />
    /// </summary>
    /// <param name="results"><seealso cref="ValidationResult" /> to conver.</param>
    /// <returns>Returns a <seealso cref="ValidationProblemDetails" /></returns>
    public static ValidationProblemDetails ToProblemDetails(this ValidationResult results) =>
        new(
            results.Errors.GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            )
        ) {
            Title = "Validation error has occurred.",
            Status = StatusCodes.Status400BadRequest,
            Type = HttpStatusCodes.Default[StatusCodes.Status400BadRequest]
        };
}