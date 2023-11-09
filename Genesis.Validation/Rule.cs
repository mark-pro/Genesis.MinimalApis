namespace Genesis.Validation;

/// <summary>
/// Base rule for validating a type
/// </summary>
public record Rule<T>(
    string PropertyName,
    Func<T, bool> Validator,
    string ErrorMessage) : IRule<T>;