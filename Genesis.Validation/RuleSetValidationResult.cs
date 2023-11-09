namespace Genesis.Validation;

/// <summary>
/// A base rulset validation to use
/// </summary>
public record struct RuleSetValidationResult<T>(
    IEnumerable<ValidationResult> Validations
) : IRuleSetValidationResult<T>;