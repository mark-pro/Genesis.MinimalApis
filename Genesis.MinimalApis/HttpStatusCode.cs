using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Genesis;

public record HttpStatusCode(int Status, Uri Type) {
    public HttpStatusCode(int status, string type) : this(status, new Uri(type)) {}

    public static implicit operator int(HttpStatusCode code) =>
        code.Status;
}

public class HttpStatusCodes : IEnumerable<HttpStatusCode>, IReadOnlyDictionary<int, string> {

    public readonly static HttpStatusCodes Default = new();
    public readonly static Seq<HttpStatusCode> StatusCodes =
        ((IEnumerable<HttpStatusCode>) Default).ToSeq();

    readonly Dictionary<int, string> _statusCodes;

    public HttpStatusCodes() {
        _statusCodes = typeof(StatusCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(fi => Optional(fi.GetValue(null)))
            .Somes()
            .Cast<int>()
            .Distinct()
            .ToDictionary(v => v, v => $"https://httpwg.org/specs/rfc9110.html#status.{v}");
    }

    public string this[int key] => _statusCodes[key];

    public IEnumerable<int> Keys => _statusCodes.Keys;

    public IEnumerable<string> Values => _statusCodes.Values;

    public int Count => _statusCodes.Count;

    public bool ContainsKey(int key) => _statusCodes.ContainsKey(key);

    public IEnumerator<HttpStatusCode> GetEnumerator() =>
        _statusCodes.Select(s => new HttpStatusCode(s.Key, s.Value)).GetEnumerator();

    public bool TryGetValue(int key , [MaybeNullWhen(false)] out string value) =>
        _statusCodes.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    IEnumerator<KeyValuePair<int , string>> IEnumerable<KeyValuePair<int , string>>.GetEnumerator() =>
        _statusCodes.GetEnumerator();
}