namespace Genesis.Validation;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ValidateAttribute : Attribute {
    
    public readonly Type Type;

    public ValidateAttribute(Type type) =>
        Type = type;

}