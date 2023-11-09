namespace Genesis.Validation;

public interface IRuleSetValidationResult<T> {
    IEnumerable<ValidationResult> Validations { get; }
    bool IsValid => Validations.All(v => v is ValidationResult.Success<T>);
}