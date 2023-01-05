namespace Genesis.Validation;

using System.Text.Json;
using System.Text.Json.Serialization;
using LanguageExt;

public class ValidatableType<VT, T> : NewType<VT, T> where VT : NewType<VT, T> {


    protected ValidatableType(T value) : base(value) {}

    public static bool TryParse(string value, out VT? type) {
        type = default;
        type = type switch {
            string => (VT?) Activator.CreateInstance(typeof(VT), value),
            _ => JsonSerializer.Deserialize<VT>(value.AsSpan(), new JsonSerializerOptions() {
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            })
        };

        return type is not null;
    }

    public static implicit operator T(ValidatableType<VT, T> type) =>
        type.Value;
}