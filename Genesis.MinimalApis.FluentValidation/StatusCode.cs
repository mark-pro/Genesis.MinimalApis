using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Genesis;

public record StatusCode(int Status, Uri Type) {
    public StatusCode(int status, string type) : this(status, new Uri(type)) {}

    public static implicit operator int(StatusCode code) =>
        code.Status;
}

public class GenesisStatusCodes : IEnumerable<StatusCode>, IReadOnlyDictionary<int, string> {

    public readonly static GenesisStatusCodes Default = new();

    readonly Dictionary<int, string> _statusCodes;

    public GenesisStatusCodes() {
        var fields = typeof(StatusCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly);

        _statusCodes = 
            fields.Select(fi => fi.GetValue(null))
            .Distinct()
            .Select(v => ((int) v!, $"https://httpwg.org/specs/rfc9110.html#status.{v}"))
            .ToDictionary(i => i.Item1, i => i.Item2);
    }

    public string this[int key] => _statusCodes[key];

    public IEnumerable<int> Keys => _statusCodes.Keys;

    public IEnumerable<string> Values => _statusCodes.Values;

    public int Count => _statusCodes.Count;

    public bool ContainsKey(int key) => _statusCodes.ContainsKey(key);

    public IEnumerator<StatusCode> GetEnumerator() =>
        _statusCodes.Select(s => new StatusCode(s.Key, s.Value)).GetEnumerator();

    public bool TryGetValue(int key , [MaybeNullWhen(false)] out string value) =>
        _statusCodes.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<KeyValuePair<int , string>> IEnumerable<KeyValuePair<int , string>>.GetEnumerator() =>
        _statusCodes.GetEnumerator();
}