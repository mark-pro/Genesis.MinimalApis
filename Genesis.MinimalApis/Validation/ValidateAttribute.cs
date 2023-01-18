namespace Genesis.Validation;

/// <summary>
/// Marks a method parameter type for validation.
/// <code>
/// [HttpPost("api/insert)]
/// [Validate(typeof(MyClass)]
/// public Insert(MyClass value) { }
/// </code>
/// Note: The attribute will be ignored if an http method attribute is not provided.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ValidateAttribute : Attribute {
    
    public readonly Type Type;

    public ValidateAttribute(Type type) =>
        Type = type;

}