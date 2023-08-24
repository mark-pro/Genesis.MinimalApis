namespace Genesis.Validation;

using System.Diagnostics;

/// <summary>
/// Performs validation on a minimal endpoint arguments paired by type
/// </summary>
/// <typeparam name="T">The type which will be used for the internal <seealso cref="IValidator" /></typeparam>
public sealed class ValidationFilter<T> : IEndpointFilter {

    readonly IValidator<T> _validator;

    /// <summary>
    /// Sets up the internal <seealso cref="IValidator" />
    /// </summary>
    /// <param name="validator">The <seealso cref="IValidator" /> to use to validate a given argument's type</param>
    public ValidationFilter(IValidator<T> validator) =>
        _validator = validator;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context , EndpointFilterDelegate next) {
        Option<object> argument = context.Arguments.Map(Optional).Somes()
            .FirstOrDefault(t => t.GetType() == typeof(T));
        
        return await argument
            .Bind(a => Try(() => (T) a).ToOption())
            .MapAsync(a =>  
                _validator.ValidateAsync(a)
                    .MapAsync(async r => r.IsValid 
                        ? await next(context) 
                        : r.ToProblemDetails().Apply(Results.Problem)))
            .IfNone(() => Results.Problem(new() {
                Title = "Validation could not be performed.",
                Status = StatusCodes.Status403Forbidden,
                Type = HttpStatusCodes.Default[StatusCodes.Status403Forbidden],
                Detail = $"Could not find validator for {typeof(T).Name}"
            }));
    }
}
