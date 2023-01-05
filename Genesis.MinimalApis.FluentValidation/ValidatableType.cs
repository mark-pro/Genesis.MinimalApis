namespace Genesis.Validation;

using System.Text.Json;
using System.Text.Json.Serialization;
using LanguageExt;

public class ValidatableType<T> : NewType<ValidatableType<T>, T> {
    public ValidatableType(T value) : base(value) {}
    public static bool TryParse(string value, out ValidatableType<T>? type) {
        type = default;
        try {
            type = type switch {
                _ when typeof(T) == typeof(string) => (ValidatableType<T>?) Activator.CreateInstance(typeof(ValidatableType<T>), value),
                _ => new ValidatableType<T>(JsonSerializer.Deserialize<T>(value.AsSpan(), new JsonSerializerOptions() {
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                }))
            };
        } catch { return false; }

        return type is not null;
    }
    public static implicit operator T(ValidatableType<T> type) =>
        type.Value;
}