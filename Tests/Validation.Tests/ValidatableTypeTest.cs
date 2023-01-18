namespace Validation.Tests;

[TestClass]
public class ValidatableTypeTest {

    record SampleObject(int Id, string Value);

    static IEnumerable<object[]> Data => new object[][] {
        new object[] { "Hello World!", typeof(string) },
        new object[] { 10, typeof(int) },
        new object[] { 10.0, typeof(double) },
        new object[] { new SampleObject(10, "foo"), typeof(SampleObject) },
        new object[] { DateTime.Now, typeof(DateTime) }
    };

    [TestMethod]
    [DynamicData(nameof(Data))]
    public void TryParseTest(object data, Type type) {
        this.GetType().GetMethod(nameof(GenericTryParseTest))
            ?.MakeGenericMethod(type)
            ?.Invoke(null, new object[] { data });

        var nr = ValidatableType<string>.TryParse(null!, out var nvt);
        nr.Should().BeFalse();
        nvt!.Should().BeNull();
    }

    static void GenericTryParseTest<T>(T data) {
        var method = typeof(ValidatableType<T>).GetMethod(nameof(ValidatableType<T>.TryParse));
        var args = new object[] { data!, null! };
        var nr = (bool) (method?.Invoke(null, args) ?? false);
        var vt = (ValidatableType<T>) args[1];
        nr.Should().BeTrue();
        ((T) vt!).Should().Be(data);
    }

}