namespace Validation.Tests;

using static System.Text.Json.JsonSerializer;

[TestClass]
public class ValidatableTypeTest {

    record SampleObject(int Id, string Value);

    const string STRING = "Hello world!";
    const int INT = 10;
    const double DOUBLE = 10.0;
    static SampleObject OBJECT => new(10, "foo");

    [TestMethod]
    public void TryParseTest() {
        var sr = ValidatableType<string>.TryParse(STRING, out var svt);
        sr.Should().BeTrue();
        ((string) svt!).Should().Be(STRING);

        var ir = ValidatableType<int>.TryParse($"{INT}", out var ivt);
        ir.Should().BeTrue();
        ((int) ivt!).Should().Be(INT);

        var dr = ValidatableType<double>.TryParse($"{DOUBLE}", out var dvt);
        dr.Should().BeTrue();
        ((double) dvt!).Should().Be(DOUBLE);

        var oJson = Serialize(OBJECT);
        var or = ValidatableType<SampleObject>.TryParse(oJson, out var ovt);
        or.Should().BeTrue();
        ((SampleObject) ovt!).Should().Be(OBJECT);
    }

}