namespace Genesis.Validation;

public sealed class ValidationFilter<T> : IEndpointFilter {

    readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator) =>
        _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context , EndpointFilterDelegate next) {
        Option<object> argument = context.Arguments.FirstOrDefault(t => t is not null && t.GetType() == typeof(T));
        return await argument
            .Map(a => Try(() => (T) a).ToOption())
            .Flatten()
            .MatchAsync(
                async a =>
                    (await _validator.ValidateAsync(a)) is var vr && vr.IsValid
                    ? await next(context)
                    : Results.Problem(vr.ToProblemDetails()),
                () => Results.Problem(new() {
                    Title = "Validation could not be performed.",
                    Status = StatusCodes.Status403Forbidden,
                    Type = GenesisStatusCodes.Default[StatusCodes.Status403Forbidden],
                    Detail = $"Could not find validator for {typeof(T).Name}"
                }
            ));
    }
}
