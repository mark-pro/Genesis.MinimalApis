namespace Genesis.Validation;

using System.Text.Json;
using System.Text.Json.Serialization;

public class ValidatableType<VT, T> where VT : ValidatableType<VT, T> {

    readonly T _value;

    protected ValidatableType(T value) =>
        _value = value;

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

    public static explicit operator T(ValidatableType<VT, T> type) =>
        type._value;
    
    public static explicit operator ValidatableType<VT, T>(T value) =>
        new(value);

    public T GetValue() => _value;

    public static bool operator ==(ValidatableType<VT, T> lhs, ValidatableType<VT, T> rhs) =>
        lhs.Equals(rhs);

    public static bool operator != (ValidatableType<VT, T> lhs, ValidatableType<VT, T> rhs) =>
        !lhs.Equals(rhs);

    public override string ToString() =>
        _value?.ToString() ?? "()";


    public override int GetHashCode() =>
        _value is null ? 0 : _value.GetHashCode();

    public override bool Equals(object? obj) {
        if (ReferenceEquals(this , obj))
            return true;

        if (obj is null) return false;

        int? hc = obj switch {
            T o => o.GetHashCode(),
            VT o => o.GetHashCode(),
            _ => null
        };

        return hc is not null && hc == GetHashCode();
    }
}