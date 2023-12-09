namespace Genesis.Validation;

/// <summary>
/// A base rule set validation to use
/// </summary>
public record struct RuleSetValidationResult<T>(
    IEnumerable<ValidationResult> Validations
) : IRuleSetValidationResult<T>;