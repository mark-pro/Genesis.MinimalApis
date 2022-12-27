namespace Genesis.Validation;

public sealed class ValidationFilter<T> : IEndpointFilter {

    readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator) =>
        _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context , EndpointFilterDelegate next) {
        var argument = context.Arguments.FirstOrDefault(t => t is not null && t.GetType() == typeof(T));
        if(argument is null) return Results.Problem(new() {
            Title = "Validation could not be performed.",
            Status = StatusCodes.Status403Forbidden,
            Type = GenesisStatusCodes.Default[StatusCodes.Status403Forbidden],
            Detail = $"Could not find validator for {typeof(T).Name}"
        });
        var foo = GenesisStatusCodes.Status400badRequest;
        var validationResult = await _validator.ValidateAsync((T) argument);

        if(!validationResult.IsValid) 
            return Results.Problem(validationResult.ToProblemDetails());

        return await next(context);
    }
}
