using Genesis.Validation;

public abstract record ValidationResult {
    public record Success<T>(T Value) : ValidationResult;
    public record Failure(string PropertyName, string Message) : ValidationResult;
}

public static partial class Core {
    /// <summary>
    /// Successful validation with the passed in value
    /// </summary>
    /// <param name="value">The value of the successful validation 
    /// <returns>A successful validation</returns>
    public static ValidationResult ValidationSucc<T>(T value) =>
        new ValidationResult.Success<T>(value);
    
    /// <summary>
    /// A failed validation with the propery name which failed and a message
    /// related to the failure
    /// </summary>
    /// <param name="propertyName">The propery name which failed validation</param>
    /// <param name="message">Additional information about the failed validation</param>
    public static ValidationResult ValidationFail(string propertyName, string message) =>
        new ValidationResult.Failure(propertyName, message);
}