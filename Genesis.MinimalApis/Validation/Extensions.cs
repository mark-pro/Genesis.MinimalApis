using Microsoft.AspNetCore.Builder;

namespace Genesis.Validation;

public static class RouteHandlerBuilderExtensions {
    public static RouteHandlerBuilder AddValidationFilter<T>(this RouteHandlerBuilder builder) =>
        builder.AddEndpointFilter<ValidationFilter<T>>();

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
            Type = GenesisStatusCodes.Default[StatusCodes.Status400BadRequest]
        };
}