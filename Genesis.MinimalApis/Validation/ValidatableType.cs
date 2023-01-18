namespace Genesis.Validation;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

public class ValidatableType<TO, T> : NewType<TO, T> where TO : ValidatableType<TO, T> {
    
    public ValidatableType(T value) : base(value) {}
    
    public static bool TryParse(string value, [MaybeNullWhen(false)] out TO type) =>
        (type = 
            (typeof(T) == typeof(string) 
            ? TryOption(() => Optional(Activator.CreateInstance(typeof(TO), value) as TO))
            : TryOption(() => {
                var jo = new JsonSerializerOptions() {
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
                return Optional(JsonSerializer.Deserialize<T>(value.AsSpan(), jo))
                .Map(New);
            }))
            .IfNoneOrFail(() => default!)
        ) is not null;

    public static implicit operator T(ValidatableType<TO, T> type) =>
        type.Value;
}