namespace Validation.Tests;

using System.Text.Json;
using static System.Text.Json.JsonSerializer;
using System.Text.Json.Serialization;

[TestClass]
public class ValidatableTypeTest {

    record SampleObject(int id, string value);

    const string STRING = "Hello world!";
    const int INT = 10;
    const double DOUBLE = 10.0;
    static SampleObject OBJECT => new(10, "foo");

    [TestMethod]
    [DynamicData(nameof(TypeData))]
    public void TryParseTest(object value, Type type) {
        var serSettings = new JsonSerializerOptions() {
            NumberHandling = JsonNumberHandling.WriteAsString
        };
        var typeArgs = new Type[] { type };
        var validatableType = typeof(ValidatableType<>);
        var vt = validatableType.MakeGenericType(typeArgs);
        var m = vt.GetMethod("TryParse")!;

        var v = value switch {
            var o when type == typeof(SampleObject) => Serialize(o),
            _ => $"{value}"
        };

        var parameters = new object[] { v, null! };
        var r = (bool) m.Invoke(null, parameters)!;
        var outVal = parameters[1];

        r.Should().BeTrue();
        outVal.Should().NotBeNull()
            .And.BeOfType(vt);

        // var sr = ValidatableType<string>.TryParse(STRING, out var svt);
        // sr.Should().BeTrue();
        // ((string) svt).Should().Be(STRING);

        // var ir = ValidatableType<int>.TryParse($"{INT}", out var ivt);
        // ir.Should().BeTrue();
        // ((int) ivt).Should().Be(INT);

        // var dr = ValidatableType<double>.TryParse($"{DOUBLE}", out var dvt);
        // dr.Should().BeTrue();
        // ((double) dvt).Should().Be(DOUBLE);

        // var ojson = Serialize(OBJECT);
        // var or = ValidatableType<SampleObject>.TryParse(ojson, out var ovt);
        // or.Should().BeTrue();
        // ((SampleObject) ovt).Should().Be(OBJECT);
    }

    static IEnumerable<object[]> TypeData => new object[][] {
        new object[] { STRING, typeof(string) },
        new object[] { INT, typeof(int) },
        new object[] { DOUBLE, typeof(double) },
        new object[] { OBJECT, typeof(SampleObject) }
    };

}