namespace Genesis.Validation;

public interface IRule<T> {
    string PropertyName { get; }
    Func<T, bool> Validator { get; }
    string? ErrorMessage { get; }
}