namespace Genesis.Validation;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ValidatableType<T> : NewType<ValidatableType<T>, T> {
    
    public ValidatableType(T value) : base(value) {}
    
    public static bool TryParse(string value, [MaybeNullWhen(false)] out ValidatableType<T> type) =>
        (type = 
            (typeof(T) == typeof(string) 
            ? TryOption(() => Optional((ValidatableType<T>?) Activator.CreateInstance(typeof(ValidatableType<T>), value)))
            : TryOption(() => {
                var jo = new JsonSerializerOptions() {
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
                return Optional(JsonSerializer.Deserialize<T>(value.AsSpan(), jo))
                .Map(New);
            }))
            .IfNoneOrFail(() => default!)
        ) is not null;

    public static implicit operator T(ValidatableType<T> type) =>
        type.Value;
}